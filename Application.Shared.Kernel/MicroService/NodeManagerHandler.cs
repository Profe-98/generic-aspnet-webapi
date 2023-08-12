using System;
using Application.Shared.Kernel.Network;
using Application.Shared.Kernel.Threading.Service;
using Application.Shared.Kernel.Threading.Task;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Configuration.Service;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table;

namespace Application.Shared.Kernel.MicroService
{
    public class NodeManagerHandler : INodeManagerHandler
    {
        #region Private
        private TaskObject taskObject;
        private readonly ISingletonNodeDatabaseHandler _databaseHandler;
        private readonly IAppconfig _appConfig;
        private readonly ITaskSchedulerBackgroundServiceQueuer _taskSchedulerBackgroundServiceQueuer;
        private NodeModel _node = null;
        private bool _isRegisterActionProceeded;

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

        }
        #endregion
        #region Methods
        public async void Register()
        {

            try
            {

                NodeModel tmpNode = new NodeModel { Uuid = _appConfig.AppServiceConfiguration.WebApiConfigurationModel.NodeUuid };
                string queryS = tmpNode.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.SELECT).ToString();
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


                NodeModel nodeModelNetworkParams = new NodeModel
                {
                    Ip = _node.Ip,
                    Port = _node.Port,
                    DnsServers = _node.DnsServers,
                    Mask = _node.Mask,
                    NetId = _node.NetId,
                    Gateway = _node.Gateway,
                };
                queryS = nodeModelNetworkParams.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.UPDATE, tmpNode).ToString();
                nodeModelNetworkParams.Uuid = tmpNode.Uuid;
                await _databaseHandler.ExecuteQueryWithMap<NodeModel>(queryS, nodeModelNetworkParams);
                _isRegisterActionProceeded = true;
            }
            catch(Exception ex) {
                //wenn keine Verbindung zum nodemanager mysql backend besteht (service: ISingletonNodeDatabaseHandler)
            }
            KeepAlive();

        }
        public async void KeepAlive()
        {
            if (!_node.IsRegistered)
            {
                taskObject = new TaskObject(async () =>
                {
                    try
                    {
                        _node.LastKeepAlive = DateTime.Now;
                        NodeModel tmpNode = new NodeModel { Uuid = _node.Uuid };


                        string query = _node.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.UPDATE, tmpNode, _node).ToString();
                        QueryResponseData queryResponseData = await _databaseHandler.ExecuteQueryWithMap<NodeModel>(query, _node);
                        if (queryResponseData.HasErrors)
                            throw new InvalidOperationException();
                    }
                    catch(Exception ex)
                    {

                    }

                }, BackendAPIDefinitionsProperties.NodeSendKeepAliveTime);
                _node.IsRegistered = true;
                _taskSchedulerBackgroundServiceQueuer.Enqueue(taskObject);
            }
        }
        public async void Unregister()
        {
            NodeModel tmpNode = new NodeModel { Uuid = _node.Uuid };
            string query = tmpNode.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.DELETE).ToString();
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
