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

using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Web.AspNet.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Application.Model.Database.MySQL.Data;
using WebApiFunction.Web.AspNet.Filter;
using WebApiFunction.Formatter;
using WebApiFunction.Web.AspNet.Healthcheck;
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
using WebApiFunction.Application.Model.Database.MySQL.Table;
using WebApiFunction.Application.Model.Database.MySQL;

namespace WebApiFunction.MicroService
{
    public class NodeManagerHandler : INodeManagerHandler
    {
        #region Private
        private TaskObject taskObject;
        private readonly ISingletonNodeDatabaseHandler _databaseHandler;
        private readonly IAppconfig _appConfig;
        private readonly ITaskSchedulerBackgroundServiceQueuer _taskSchedulerBackgroundServiceQueuer;
        private NodeModel _node = null;

        public NodeModel NodeModel => _node;
        #endregion
        #region Ctor
        public NodeManagerHandler(ISingletonNodeDatabaseHandler databaseHandler, ITaskSchedulerBackgroundServiceQueuer taskSchedulerBackgroundServiceQueuer, IAppconfig appconfig)
        {
            _appConfig = appconfig;
            _databaseHandler = databaseHandler;
            _taskSchedulerBackgroundServiceQueuer = taskSchedulerBackgroundServiceQueuer;
            var envVars = Environment.GetEnvironmentVariables();


            if (_appConfig.AppServiceConfiguration.WebApiConfigurationModel.NodeUuid == Guid.Empty)
            {
                _appConfig.AppServiceConfiguration.WebApiConfigurationModel.NodeUuid = Guid.NewGuid();
                _appConfig.Save();
            }
            string[] split = null;
            if(envVars.Contains("ASPNETCORE_URLS"))
            {
                split = envVars["ASPNETCORE_URLS"].ToString().Split(':');
            }
            int port = 0;
            if(split != null&&split.Length > 0&& split.Length>2) 
            {
                port = int.Parse(split[2]); 
            }
            var ipInfo = NetworkUtilityHandler.GetPhysicalEthernetIPAdress();
            string gwDataStr = null;
            string dnsDataStr = null;
            if(ipInfo.Gateway != null)
            {

                foreach (var item in ipInfo.Gateway)
                {
                    gwDataStr += item.Address.ToString() + ";";
                }
            }
            if (ipInfo.DnsServers!= null)
            {

                foreach (var item in ipInfo.DnsServers)
                {
                    dnsDataStr += item.ToString() + ";";
                }
            }
            _node = new NodeModel()
            {

                Ip = ipInfo.Ip.ToString(),
                Gateway = gwDataStr,
                NetId = ipInfo.NetId.ToString(),
                Mask = ipInfo.Mask.ToString(),
                DnsServers = dnsDataStr,
                Port = port,
                LastKeepAlive = DateTime.Now,
                Uuid = _appConfig.AppServiceConfiguration.WebApiConfigurationModel.NodeUuid
            };
            Register();

        }
        #endregion
        #region Methods
        public async void Register()
        {

            NodeModel tmpNode = new NodeModel { Uuid = _appConfig.AppServiceConfiguration.WebApiConfigurationModel.NodeUuid };
            string queryS = tmpNode.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT).ToString();
            QueryResponseData<NodeModel> queryResponseDataS = await _databaseHandler.ExecuteQueryWithMap<NodeModel>(queryS, tmpNode);
            if (!queryResponseDataS.HasStorageData)
            {
                throw new NotImplementedException("node is not found in database, that means the node must be registered in database and the uuid must be stored in appserviceconfiguration.json file for authentification of the node with the backend");

            }
            NodeModel currentDataFromDb = queryResponseDataS.FirstRow;
            if (_node.NodeTypeUuid != currentDataFromDb.NodeTypeUuid)
                _node.NodeTypeUuid = currentDataFromDb.NodeTypeUuid;

            if (_node.Name != currentDataFromDb.Name)
                _node.Name = currentDataFromDb.Name;


            NodeModel nodeModelNetworkParams = new NodeModel { 
             Ip=_node.Ip,
             Port=_node.Port,
             DnsServers=_node.DnsServers,
             Mask= _node.Mask,
             NetId=_node.NetId,
             Gateway= _node.Gateway,
            };
            queryS = nodeModelNetworkParams.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.UPDATE, tmpNode).ToString();
            nodeModelNetworkParams.Uuid = tmpNode.Uuid;
            await _databaseHandler.ExecuteQueryWithMap<NodeModel>(queryS, nodeModelNetworkParams);

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
