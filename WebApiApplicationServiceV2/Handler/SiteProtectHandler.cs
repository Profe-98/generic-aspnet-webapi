using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;

using WebApiApplicationService.InternalModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using WebApiFunction.Mail;
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Controller.Modules;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Database.MySQL;
using WebApiFunction.Database.MySQL.Data;
using WebApiFunction.Filter;
using WebApiFunction.Formatter;
using WebApiFunction.LocalSystem.IO.File;
using WebApiFunction.Log;
using WebApiFunction.Metric;
using WebApiFunction.Metric.Influxdb;
using WebApiFunction.MicroService;
using WebApiFunction.Network;
using WebApiFunction.Security;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading;
using WebApiFunction.Threading.Service;
using WebApiFunction.Threading.Task;
using WebApiFunction.Utility;
using WebApiFunction.Web;
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;
using WebApiFunction.Healthcheck;
using WebApiFunction.Application;
using WebApiFunction.Web.Authentification.JWT;

namespace WebApiApplicationService.Handler
{
    public class SiteProtectHandler : IScopedSiteProtectHandler, ISingletonSiteProtectHandler
    {
        private readonly IAppconfig _appConfig;

        public SiteProtectHandler(IAppconfig appconfig)
        {
            _appConfig = appconfig;
        }

        public async Task<ObserveResult> ObserveEndpoint(HttpContext context, IPEndPoint endPoint, IDatabaseHandler _databaseHandler)
        {
            string urlPath = context.Request.Path.Value;
            ObserveResult observeResult = new ObserveResult();
            UserInformation userInformation = new UserInformation(endPoint);
            Guid key = userInformation.RequestUuid;

            SiteProtectModel siteProtectModel = new SiteProtectModel();
            siteProtectModel.Uuid = key;
            if (urlPath.Length >= _appConfig.AppServiceConfiguration.ApiSecurityConfigurationModel.SiteProtect.MaxHttpRequUriLen)
            {
                observeResult.IsUriTooLong = true;
            }
            else
            {
                long contentLength = context.Request.Body.Length;
                long httpRequContentLenghtHeaderValue = 0;
                bool foundContentLengthInHttpRequHeader = false;
                foreach (string headerKey in context.Request.Headers.Keys)
                {
                    if(headerKey.Length < _appConfig.AppServiceConfiguration.ApiSecurityConfigurationModel.SiteProtect.MaxHttpHeaderFieldLen)
                    {
                        StringValues val = context.Request.Headers[headerKey];

                        for (int i = 0; i < val.Count; i++)
                        {
                            if (val[i].Length >= _appConfig.AppServiceConfiguration.ApiSecurityConfigurationModel.SiteProtect.MaxHttpHeaderFieldLen)
                            {
                                observeResult.IsHeaderFieldValueTooLong = true;
                                break;
                            }
                        }
                        if (headerKey.ToLower() == "content-length" && !foundContentLengthInHttpRequHeader)
                        {
                            if (val.Count == 1)
                            {
                                if (long.TryParse(val[0], out httpRequContentLenghtHeaderValue))
                                {
                                    foundContentLengthInHttpRequHeader = true;
                                }
                            }
                        }
                    }
                    else
                    {

                        observeResult.IsHeaderFieldTooLong = true;
                        break;
                    }
                }

                observeResult.IsContentLenghtGiven = foundContentLengthInHttpRequHeader;
                if (contentLength != 0)
                {
                    if (contentLength >= _appConfig.AppServiceConfiguration.ApiSecurityConfigurationModel.SiteProtect.MaxHttpContentLen)
                    {
                        observeResult.IsPayloadTooLarge = true;
                    }
                    if (httpRequContentLenghtHeaderValue != 0 && httpRequContentLenghtHeaderValue != contentLength)
                    {
                        observeResult.ContentLenAndBodyLenUnequal = true;
                    }
                }
            }

            if (userInformation == null)
                userInformation = new UserInformation(endPoint);


            string query = siteProtectModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, siteProtectModel).ToString();
            QueryResponseData<SiteProtectModel> queryResponseData = await _databaseHandler.ExecuteQueryWithMap<SiteProtectModel>(query, siteProtectModel);
            if (queryResponseData.HasSuccess && queryResponseData.HasData)
            {
                userInformation.BanTime = queryResponseData.DataStorage[0].BanTime;
            }

            siteProtectModel.IpAddr = endPoint.Address.ToString();
            if (!userInformation.HasActiveBan)
            {
                if(userInformation.IsBanTimeOver)
                {
                    userInformation.ApiCalls.Clear();
                    userInformation.TimeBetweenLastRequestViolationCount = 0;

                    query = siteProtectModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.DELETE,siteProtectModel).ToString();
                    QueryResponseData queryResponseDataDelete = await _databaseHandler.ExecuteQuery(query);
                }

                string userAgent = context.HeaderValueGet("user-agent", false);
                bool noUserAgentInHeader = String.IsNullOrEmpty(userAgent) ?
                    true : false;

                if (!noUserAgentInHeader)
                    userInformation.HandleUserAgents(userAgent);
                else
                {
                    userInformation.NoUserAgentRequestsCount++;
                    if (userInformation.MaximumRequestWithoutUserAgentExceeded)
                        userInformation.SetBanTime(BackendAPIDefinitionsProperties.SiteProtectorBanTime);
                }

                userInformation.IncreaseRequestCallCount(urlPath);
                bool checkPathRequestTimeFromPathRequest = userInformation.CheckTimeBetweenRequestViolationFromPath(urlPath);
                if (userInformation.MaxRequestPerHourExceeded)
                {
                    userInformation.SetBanTime(BackendAPIDefinitionsProperties.SiteProtectorBanTime);
                }
                if(userInformation.HasActiveBan)
                {
                    siteProtectModel.BanTime = userInformation.BanTime;
                    query = siteProtectModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT, siteProtectModel).ToString();
                    QueryResponseData queryResponseDataInsert = await _databaseHandler.ExecuteQuery<SiteProtectModel>(query,siteProtectModel);
                }

            }
            observeResult.UserInformation = userInformation;
            return observeResult;
        }
        public class ObserveResult
        {
            public UserInformation UserInformation { get; set; }
            public bool IsUriTooLong { get; set; }
            public bool IsHeaderFieldTooLong { get; set; }
            public bool IsHeaderFieldValueTooLong { get; set; }
            public bool IsContentLenghtGiven { get; set; }
            public bool IsPayloadTooLarge { get; set; }
            public bool ContentLenAndBodyLenUnequal { get; set; }

            public bool CanPerformAction
            {
                get
                {
                    return !IsUriTooLong && !IsHeaderFieldTooLong && !IsHeaderFieldValueTooLong && !(IsContentLenghtGiven && (IsPayloadTooLarge || ContentLenAndBodyLenUnequal));
                }
            }

        }
        public class UserInformation
        {
            public readonly IPEndPoint WANSocket;
            private Guid _internalId = Guid.Empty;
            public Dictionary<string, UserInformation> LastApiControllerCall = new Dictionary<string, UserInformation>();

            public Guid RequestUuid
            {
                get
                {
                    if(_internalId == Guid.Empty && WANSocket != null)
                    {
                        string wanSocket = WANSocket.ToString();
                        using (EncryptionHandler encryp = new EncryptionHandler())
                        {
                            string md5 = encryp.MD5(wanSocket);
                            byte[] bytes = Encoding.Default.GetBytes(md5);
                            _internalId = new Guid(md5);
                        }
                    }
                    return _internalId;
                }
                set
                {
                    _internalId = value;
                }
            }
            
            public DateTime BanTime { get; set; } = DateTime.MinValue;
            public bool HasActiveBan
            {
                get
                {
                    return this.BanTime > DateTime.Now?
                        true:false;
                }
            }
            public bool IsBanTimeOver
            {
                get
                {
                    return this.BanTime != DateTime.MinValue && this.BanTime < DateTime.Now;
                }
            }
            public List<string> UserAgents { get; set; } = null;

            public long LastApiCall { get; set; }

            /// <summary>
            /// Key is the Hour e.g. 4 and the Value of the Key is the count of request per hour (key)
            /// </summary>
            public Dictionary<int, int> ApiCalls { get; set; } = new Dictionary<int, int>();

            public bool MaxRequestPerHourExceeded
            {
                get
                {
                    return ApiCalls.TryGetValue(DateTime.Now.Hour, out int counter) ?
                        counter >= BackendAPIDefinitionsProperties.MaxRequestPerHour : false;
                }
            }


            /// <summary>
            /// True when the Time ago after last request is smaller than granted for user
            /// </summary>
            public bool HasTimeBetweenLastRequestViolation
            {
                get
                {
                    long callBefore = LastApiCall;
                    long callCurrent = DateTime.Now.Ticks;

                    LastApiCall = callCurrent;
                    return CheckTimeDiff(callBefore,callCurrent);
                }
            }
            /// <summary>
            /// Count for User that counts upward for each TimeBetweenLastRequest Violation
            /// </summary>
            public int TimeBetweenLastRequestViolationCount { get; set; }

            /// <summary>
            /// Count for Request without Useragent, increase for each Requ. w/out User-Agent in HTTP-Header
            /// </summary>
            public int NoUserAgentRequestsCount { get; set; }

            public bool MaximumRequestWithoutUserAgentExceeded
            {
                get
                {
                    return this.NoUserAgentRequestsCount >= BackendAPIDefinitionsProperties.MaxRequestWithoutUserAgent;
                }
            }
            private UserInformation()
            {

            }
            public UserInformation(IPEndPoint wan)
            {
                WANSocket = wan;
            }

            public bool HandleUserAgents(string userAgent)
            {
                if (this.UserAgents == null)
                    this.UserAgents = new List<string>();

                if(!String.IsNullOrEmpty(userAgent))
                {
                    userAgent = userAgent.ToLower();
                    if (this.UserAgents.IndexOf(userAgent) == GeneralDefs.NotFoundResponseValue)
                    {
                        this.UserAgents.Add(userAgent);
                        return true;
                    }
                }
                return false;
            }
            public static UserInformation GetEmptyObj()
            {
                return new UserInformation();
            }
            public void IncreaseRequestCallCount(string urlPath)
            {
                int currentHour = DateTime.Now.Hour;
                string key = urlPath.ToLower();
                UserInformation userInformation = UserInformation.GetEmptyObj();
                if (!LastApiControllerCall.ContainsKey(key))
                {
                    LastApiControllerCall.Add(key, userInformation) ;
                }
                if (!ApiCalls.ContainsKey(currentHour))
                {
                    ApiCalls.Clear();//damit nur die aktuelle Stunde im Dict. ist und eine potenzieller Angreifer kein Heap-Overflow machen kann
                    ApiCalls.Add(currentHour, 1);
                }
                else
                {
                    ApiCalls[currentHour]++;
                }
            }
            public bool CheckTimeBetweenRequestViolationFromPath(string urlPath)
            {
                if(!String.IsNullOrEmpty(urlPath))
                {
                    string key = urlPath.ToLower();
                    if (LastApiControllerCall.TryGetValue(key, out UserInformation value))
                    {
                        long callBefore = value.LastApiCall;

                        long callCurrent = DateTime.Now.Ticks;
                        LastApiControllerCall[key].LastApiCall = callCurrent;
                        bool diffTimeResponse = CheckTimeDiff(callBefore, callCurrent);
                        if(diffTimeResponse)
                        {
                            LastApiControllerCall[key].TimeBetweenLastRequestViolationCount++;
                            if (LastApiControllerCall[key].TimeBetweenLastRequestViolationCount >= BackendAPIDefinitionsProperties.MaxTimeBetweenRequestViolations)
                                this.SetBanTime(BackendAPIDefinitionsProperties.SiteProtectorBanTime);
                        }
                        return diffTimeResponse;
                    }
                }
                return false;
            }

            public void SetBanTime(TimeSpan timeSpan)
            {
                this.BanTime = DateTime.Now + timeSpan;

            }

            private bool CheckTimeDiff(long pastTicks,long currentTicks)
            {
                TimeSpan timeSpan1 = new TimeSpan(pastTicks);
                TimeSpan timeSpan2 = new TimeSpan(currentTicks);

                TimeSpan diff = timeSpan2 - timeSpan1;
                if (diff.TotalMilliseconds < BackendAPIDefinitionsProperties.MaxGrantedTimeToNextRequest)
                    return true;

                return false;
            }
        }
    }
}
