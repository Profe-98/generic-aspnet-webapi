using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Runtime;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Database.MySQL;
using WebApiFunction.Database.MySQL.Data;
using WebApiFunction.Filter;
using WebApiFunction.Formatter;
using WebApiFunction.Healthcheck;
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

namespace WebApiFunction.MicroService
{
    public class NodeManagerHandler : INodeManagerHandler
    {
        #region Private
        private TaskObject taskObject;
        private readonly ISingletonDatabaseHandler _databaseHandler;
        private readonly IWebHostEnvironment _env;
        private readonly IAppconfig _appConfig;
        private readonly ITaskSchedulerBackgroundServiceQueuer _taskSchedulerBackgroundServiceQueuer;
        private NodeModel _node = null;

        public NodeModel NodeModel => _node;
        #endregion
        #region Ctor
        public NodeManagerHandler(ISingletonDatabaseHandler databaseHandler, IWebHostEnvironment env, ITaskSchedulerBackgroundServiceQueuer taskSchedulerBackgroundServiceQueuer, IAppconfig appconfig)
        {
            _appConfig = appconfig;
            _databaseHandler = databaseHandler;
            _env = env;
            _taskSchedulerBackgroundServiceQueuer = taskSchedulerBackgroundServiceQueuer;
            var envVars = Environment.GetEnvironmentVariables();


            if (_appConfig.AppServiceConfiguration.WebApiConfigurationModel.NodeUuid == Guid.Empty)
            {
                _appConfig.AppServiceConfiguration.WebApiConfigurationModel.NodeUuid = Guid.NewGuid();
                _appConfig.Save();
            }
            string[] split = envVars["ASPNETCORE_URLS"].ToString().Split(':');
            var ipInfo = NetworkUtilityHandler.GetPhysicalEthernetIPAdress();
            string gwDataStr = null;
            string dnsDataStr = null;
            foreach (var item in ipInfo.Gateway)
            {
                gwDataStr += item.Address.ToString() + ";";
            }
            foreach (var item in ipInfo.DnsServers)
            {
                dnsDataStr += item.ToString() + ";";
            }
            _node = new NodeModel()
            {
                Name = env.ApplicationName,
                Ip = ipInfo.Ip.ToString(),
                Gateway = gwDataStr,
                NetId = ipInfo.NetId.ToString(),
                Mask = ipInfo.Mask.ToString(),
                DnsServers = dnsDataStr,
                Port = int.Parse(split[2]),
                LastKeepAlive = DateTime.Now,
                Uuid = _appConfig.AppServiceConfiguration.WebApiConfigurationModel.NodeUuid
            };

        }
        #endregion
        #region Methods
        public async void Register(Guid typeUuid)
        {



            _node.NodeTypeUuid = typeUuid;
            NodeModel tmpNode = new NodeModel { Uuid = _appConfig.AppServiceConfiguration.WebApiConfigurationModel.NodeUuid };
            string queryS = tmpNode.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT).ToString();
            QueryResponseData<NodeModel> queryResponseDataS = await _databaseHandler.ExecuteQueryWithMap<NodeModel>(queryS, tmpNode);
            if (!queryResponseDataS.HasStorageData)
            {
                string query = _node.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                QueryResponseData queryResponseData = await _databaseHandler.ExecuteQueryWithMap<NodeModel>(query, _node);
                if (queryResponseData.HasErrors)
                    throw new InvalidOperationException();


                queryResponseDataS = await _databaseHandler.ExecuteQueryWithMap<NodeModel>(queryS, tmpNode);

            }
            NodeModel currentDataFromDb = queryResponseDataS.FirstRow;
            if (_node.Ip == currentDataFromDb.Ip)
                _node.Ip = currentDataFromDb.Ip;
            if (_node.Port == currentDataFromDb.Port)
                _node.Port = currentDataFromDb.Port;
            if (_node.DnsServers == currentDataFromDb.DnsServers)
                _node.DnsServers = currentDataFromDb.DnsServers;
            if (_node.Gateway == currentDataFromDb.Gateway)
                _node.Gateway = currentDataFromDb.Gateway;
            if (_node.NetId == currentDataFromDb.NetId)
                _node.NetId = currentDataFromDb.NetId;
            if (_node.Mask == currentDataFromDb.Mask)
                _node.Mask = currentDataFromDb.Mask;
            if (_node.Name == currentDataFromDb.Name)
                _node.Name = currentDataFromDb.Name;


            KeepAlive();
        }
        public async void KeepAlive()
        {
            if (!_node.IsRegistered)
            {
                taskObject = new TaskObject(async () =>
                {
                    _node.LastKeepAlive = DateTime.Now;
                    NodeModel tmpNode = new NodeModel { Uuid = _node.Uuid };


                    string query = _node.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.UPDATE, tmpNode, _node).ToString();
                    QueryResponseData queryResponseData = await _databaseHandler.ExecuteQueryWithMap<NodeModel>(query, _node);
                    if (queryResponseData.HasErrors)
                        throw new InvalidOperationException();

                }, BackendAPIDefinitionsProperties.NodeSendKeepAliveTime);
                _node.IsRegistered = true;
                _taskSchedulerBackgroundServiceQueuer.Enqueue(taskObject);
            }
        }
        public async void Unregister()
        {
            NodeModel tmpNode = new NodeModel { Uuid = _node.Uuid };
            string query = tmpNode.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.DELETE).ToString();
            QueryResponseData queryResponseData = await _databaseHandler.ExecuteQueryWithMap<NodeModel>(query, tmpNode);
            if (queryResponseData.HasErrors)
                throw new InvalidOperationException();

            taskObject.CancellationTokenInstance.Cancel();
        }
        public NodeModel GetCurrentNodeData()
        {
            return _node;
        }
        #endregion
    }

}
