using System;
using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using MySql.Data.MySqlClient;
using WebApiFunction.Data.Web.MIME;
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
using WebApiFunction.Data.Web;
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
using MySqlX.XDevAPI;

namespace WebApiFunction.Ampq.Rabbitmq
{
    public class RabbitMqHandler : IRabbitMqHandler, IDisposable
    {
        public class RabbitMqSessionObject
        {
            public Func<MessageModel, MessageModelResponse> ConsumeAction { get; set; }
            public Action OkMsgAct { get; set; }
            public IModel Session { get; set; }
            public RabbitMqSessionObject(Func<MessageModel, MessageModelResponse> func, Action okActMsg,IModel session)
            {
                ConsumeAction=func;
                OkMsgAct=okActMsg;
                Session=session;
            }
        }
        private readonly ILogger _logger;
        private IConnection _connection;
        private readonly IAppconfig _appConfig;
        private readonly ISingletonJsonHandler _singletonJsonHandler;
        private readonly INodeManagerHandler _nodeManagerHandler;
        private readonly ISingletonEncryptionHandler _singletonEncryptionHandler;
        private static Dictionary<KeyValuePair<string, string>, RabbitMqSessionObject> _ = new Dictionary<KeyValuePair<string, string>, RabbitMqSessionObject>();

        public EventingBasicConsumer ConsumerEvent;

        public IConnection Connection => _connection;


        public RabbitMqHandler(ILogger<RabbitMqHandler> logger, IAppconfig appconfig, ISingletonJsonHandler singletonJsonHandler, INodeManagerHandler nodeManagerHandler, ISingletonEncryptionHandler singletonEncryptionHandler)
        {
            _logger = logger;
            _appConfig = appconfig;
            if(_appConfig != null && _appConfig.AppServiceConfiguration.RabbitMqConfigurationModel == null)
            {
                throw new Exception("rabbitmq handler cant be used when no config is given");
            }
            _singletonJsonHandler = singletonJsonHandler;
            _nodeManagerHandler = nodeManagerHandler;
            _singletonEncryptionHandler = singletonEncryptionHandler;

            _connection = GetConnection();
            if (_connection.IsOpen)
            {
                _logger.LogDebug("rabbitmq connection established");
            }
            _ = new Dictionary<KeyValuePair<string, string>, RabbitMqSessionObject>();
        }

        public IConnection GetConnection(EventHandler<ConnectionBlockedEventArgs> connectionBlockedEventHandler = null,
            EventHandler<CallbackExceptionEventArgs> callBackExceptionEventHandler = null,
            EventHandler<EventArgs> connectionUnblockedEventHandler = null,
            EventHandler<ShutdownEventArgs> shutDownEventHandler = null)
        {
            _logger.LogDebug("try to connect to rabbitmq");
            var factory = new ConnectionFactory()
            {
                ClientProvidedName= _nodeManagerHandler.NodeModel.Name,
                HostName = _appConfig.AppServiceConfiguration.RabbitMqConfigurationModel.Host,
                UserName = _appConfig.AppServiceConfiguration.RabbitMqConfigurationModel.User,
                Password = _appConfig.AppServiceConfiguration.RabbitMqConfigurationModel.Password,
                VirtualHost = _appConfig.AppServiceConfiguration.RabbitMqConfigurationModel.VirtualHost,
                Port = (int)_appConfig.AppServiceConfiguration.RabbitMqConfigurationModel.Port,
                RequestedHeartbeat = new TimeSpan(0, 0, 0, 0, _appConfig.AppServiceConfiguration.RabbitMqConfigurationModel.HeartBeatMs)
            };
            var connection = factory.CreateConnection();
            connection.ConnectionBlocked += connectionBlockedEventHandler == null ? Connection_ConnectionBlocked : connectionBlockedEventHandler;
            connection.ConnectionShutdown += shutDownEventHandler == null ? Connection_ConnectionShutdown : shutDownEventHandler;
            connection.ConnectionUnblocked += connectionUnblockedEventHandler == null ? Connection_ConnectionUnblocked : connectionUnblockedEventHandler;
            connection.CallbackException += callBackExceptionEventHandler == null ? Connection_CallbackException : callBackExceptionEventHandler;
            return connection;
        }

        private void Connection_CallbackException(object sender, CallbackExceptionEventArgs e)
        {

            _logger.LogDebug("rabbitmq callbackexception");
        }

        private void Connection_ConnectionUnblocked(object sender, EventArgs e)
        {

            _logger.LogDebug("rabbitmq connection unblocked");
        }

        private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {

            _logger.LogDebug("rabbitmq connection shutdown");
        }

        private void Connection_ConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {

            _logger.LogDebug("rabbitmq connection blocked");
        }

        public IModel GetChannel(IConnection connection = null)
        {
            if (connection == null)
                return _connection.CreateModel();
            else
            {
                return connection.CreateModel();
            }
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void SubscibeExchange(string exchangeName, Func<MessageModel, MessageModelResponse> consumeAction, Action okAct, string routingKey = "")
        {
            var _queueBind = RegisterExchange(exchangeName, consumeAction, okAct, routingKey);
        }

        private ValueTuple<IModel, QueueDeclareOk> RegisterExchange(string exchangeName, Func<MessageModel, MessageModelResponse> consumeAction,Action okAct, string routingKey = "")
        {

            var key = new KeyValuePair<string, string>(exchangeName, routingKey);
            if (!_.ContainsKey(key))
            {
                _.Add(key, new RabbitMqSessionObject(consumeAction, okAct, _connection.CreateModel()));
                _logger.LogDebug("subscription registered: " + key.ToString() + ""+(consumeAction ==null?"": " with consume action '" + consumeAction.GetType().FullName + "'") +"");
            }


            _[key].Session.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);
            _logger.LogDebug("exchange declare '" + exchangeName + "'");


            var queueName = _[key].Session.QueueDeclare(exchangeName, true, false, autoDelete: false);
            _[key].Session.QueueBind(queue: queueName,
                              exchange: queueName,
                              routingKey: routingKey);
            _logger.LogDebug("exchange-queue bind '" + queueName + "'");

            if (ConsumerEvent == null)
            {

                _logger.LogDebug("exchange-consumer registered for queue '" + queueName + "'");
                ConsumerEvent = new EventingBasicConsumer(_[key].Session);
                ConsumerEvent.Received += Consumer_Received;
                _[key].Session.BasicConsume(queueName, true, ConsumerEvent);
            }
            return new(_[key].Session, queueName);
        }
        public void PublishObject(string exchangeName, object entity, string routingKey, string message, Func<MessageModel, MessageModelResponse> consumeAction, Action okAct)
        {
            var entT = entity.GetType().FullName;
            var objJson = _singletonJsonHandler.JsonSerialize(entity);
            var msgModel = new MessageModel { CreateTimestamp = DateTime.Now.Ticks, DataSerialized = objJson, DataType = entT, Message = message, NodeId = _nodeManagerHandler.NodeModel.Uuid };
            var jsonMsg = _singletonJsonHandler.JsonSerialize(msgModel);
            var sha256 = _singletonEncryptionHandler.SHA256(jsonMsg);
            msgModel.DataHash = sha256;
            msgModel.DataHashAlgo = "sha256";

            _logger.LogDebug("publish to exchange: '" + exchangeName + "', routingKey: '" + routingKey + "', message: '" + message + "', entityType: '" + entT + "'");
            this.Publish(exchangeName, msgModel,  null,okAct, routingKey: routingKey);
        }

        public void Publish(string exchangeName, MessageModel msg, Func<MessageModel, MessageModelResponse> consumeAction,Action okAct, string routingKey = "")
        {
            try
            {
                var _queueBind = RegisterExchange(exchangeName, consumeAction, okAct, routingKey);

                var jsonSerialize = _singletonJsonHandler.JsonSerialize(msg);
                var body = Encoding.UTF8.GetBytes(jsonSerialize);
                var prop = _queueBind.Item1.CreateBasicProperties();
                
                _queueBind.Item1.BasicPublish(_queueBind.Item2, routingKey: routingKey, prop, body);

            }
            catch (Exception ex)
            {

            }

        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            MessageModel messageModel = null;
            if (e.Body.Length != 0)
            {
                _logger.LogDebug("try to consume message");

                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogDebug("raw consumed message '" + message + "'");
                messageModel = _singletonJsonHandler.JsonDeserialize<MessageModel>(message);
            }
            if (messageModel == null)
            {

                _logger.LogDebug("deserialized message was null, continue");
                return;
            }

            /*if (messageModel.NodeId == _nodeManagerHandler.NodeModel.Uuid)
                return;*/

            var key = new KeyValuePair<string, string>(e.Exchange, e.RoutingKey);
            if (_.ContainsKey(key))
            {
                _logger.LogDebug("consumer: exchange consume action is registered '" + key.ToString() + "'");
                if (_[key] != null)
                {

                    _logger.LogDebug("consumer: exchange consume action is registered '" + key.ToString() + "', action: '" + _[key].GetType().FullName + "'");

                    string classFullName = messageModel.DataType;
                    if (!string.IsNullOrEmpty(classFullName))
                    {
                        _logger.LogDebug("deserialized message was null, continue");
                        var classType = Type.GetType(classFullName);
                        object instance = classType == null ? null : Activator.CreateInstance(classType);
                        if (instance != null && _[key].ConsumeAction != null)
                        {

                            var obj = _singletonJsonHandler.JsonDeserialize(messageModel.DataSerialized, classType, null);
                            messageModel.DataDeserialized = obj;
                            var data = _[key];
                            var response = data.ConsumeAction(messageModel);
                            if (response != null)
                            {
                                var messageProceedReturnModel = (MessageModelResponse)response;
                                if (messageProceedReturnModel is MessageOK)
                                {
                                    if(data.OkMsgAct !=null)
                                    {
                                        Task.Run(data.OkMsgAct);
                                    }
                                    _logger.LogDebug("consumer: exchange consume action '" + key.ToString() + "', action '" + _[key].GetType().FullName + "' invoke response was 'MessageOK'");

                                }
                                else if (messageProceedReturnModel is MessageNOK)
                                {
                                    _logger.LogDebug("consumer: exchange consume action '" + key.ToString() + "', action '" + _[key].GetType().FullName + "' invoke response was 'MessageNOK'");

                                }
                                else
                                {
                                    _logger.LogDebug("consumer: exchange consume action '" + key.ToString() + "', action '" + _[key].GetType().FullName + "' invoke response was undefined (runtimeType: '" + messageProceedReturnModel.GetType().FullName + "')");

                                }
                            }
                            else
                            {
                                _logger.LogDebug("consumer: exchange consume action '" + key.ToString() + "', action '" + _[key].GetType().FullName + "' invoke response was null");


                            }
                             
                        }
                        else
                        {
                            _logger.LogDebug("consumer: exchange consume action '" + key.ToString() + "', action '" + _[key].GetType().FullName + "' given data-type is not a member of this assembly, deserialization not possible, abort");

                        }
                    }
                    else
                    {

                        _logger.LogDebug("consumer: exchange consume action '" + key.ToString() + "', action '" + _[key].GetType().FullName + "' has no defined data-type");
                    }
                }
                else
                {

                    _logger.LogDebug("consumer: exchange consume action is not registered '" + key.ToString() + "'");
                }
            }
            else
            {
                _logger.LogDebug("consumer: exchange consume key is not registered '" + key.ToString() + "'");

            }
        }

    }


    public static class RabbitMqExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IRabbitMqHandler, RabbitMqHandler>();
            return serviceCollection;
        }
    }
}
