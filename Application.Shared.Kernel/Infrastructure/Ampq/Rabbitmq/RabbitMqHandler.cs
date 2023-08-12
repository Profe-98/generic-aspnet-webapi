using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using Application.Shared.Kernel.MicroService;
using Application.Shared.Kernel.Security.Encryption;
using Application.Shared.Kernel.Configuration.Service;
using Application.Shared.Kernel.Infrastructure.Ampq.Rabbitmq.Data;


namespace Application.Shared.Kernel.Infrastructure.Ampq.Rabbitmq
{
    public class RabbitMqHandler : IRabbitMqHandler, IDisposable
    {
        public class RabbitMqSessionObject
        {
            public Func<MessageModel, MessageModelResponse> ConsumeAction { get; set; }
            public Action OkMsgAct { get; set; }
            public IModel Session { get; set; }
            public RabbitMqSessionObject(Func<MessageModel, MessageModelResponse> func, Action okActMsg, IModel session)
            {
                ConsumeAction = func;
                OkMsgAct = okActMsg;
                Session = session;
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
            if (_appConfig != null && _appConfig.AppServiceConfiguration.RabbitMqConfigurationModel == null)
            {
                throw new Exception("rabbitmq handler cant be used when no config is given");
            }
            _singletonJsonHandler = singletonJsonHandler;
            _nodeManagerHandler = nodeManagerHandler;
            _singletonEncryptionHandler = singletonEncryptionHandler;

            _connection = GetConnection();
            if (_connection != null && _connection.IsOpen)
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
            IConnection connection = null;
            try
            {

                var factory = new ConnectionFactory()
                {
                    ClientProvidedName = _nodeManagerHandler.NodeModel.Name,
                    HostName = _appConfig.AppServiceConfiguration.RabbitMqConfigurationModel.Host,
                    UserName = _appConfig.AppServiceConfiguration.RabbitMqConfigurationModel.User,
                    Password = _appConfig.AppServiceConfiguration.RabbitMqConfigurationModel.Password,
                    VirtualHost = _appConfig.AppServiceConfiguration.RabbitMqConfigurationModel.VirtualHost,
                    Port = (int)_appConfig.AppServiceConfiguration.RabbitMqConfigurationModel.Port,
                    RequestedHeartbeat = new TimeSpan(0, 0, 0, 0, _appConfig.AppServiceConfiguration.RabbitMqConfigurationModel.HeartBeatMs)
                };
                connection = factory.CreateConnection();
                connection.ConnectionBlocked += connectionBlockedEventHandler == null ? Connection_ConnectionBlocked : connectionBlockedEventHandler;
                connection.ConnectionShutdown += shutDownEventHandler == null ? Connection_ConnectionShutdown : shutDownEventHandler;
                connection.ConnectionUnblocked += connectionUnblockedEventHandler == null ? Connection_ConnectionUnblocked : connectionUnblockedEventHandler;
                connection.CallbackException += callBackExceptionEventHandler == null ? Connection_CallbackException : callBackExceptionEventHandler;
            }
            catch (Exception ex)
            {

                _logger.LogDebug("connection failed");
            }
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
            if (Connection == null)
                return;
            var _queueBind = RegisterExchange(exchangeName, consumeAction, okAct, routingKey);
        }

        private ValueTuple<IModel, QueueDeclareOk> RegisterExchange(string exchangeName, Func<MessageModel, MessageModelResponse> consumeAction, Action okAct, string routingKey = "")
        {

            var key = new KeyValuePair<string, string>(exchangeName, routingKey);
            if (!_.ContainsKey(key))
            {
                _.Add(key, new RabbitMqSessionObject(consumeAction, okAct, _connection.CreateModel()));
                _logger.LogDebug("subscription registered: " + key.ToString() + "" + (consumeAction == null ? "" : " with consume action '" + consumeAction.GetType().FullName + "'") + "");
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
            if (Connection == null)
                return;
            var entT = entity.GetType().FullName;
            var objJson = _singletonJsonHandler.JsonSerialize(entity);
            var msgModel = new MessageModel { CreateTimestamp = DateTime.Now.Ticks, DataSerialized = objJson, DataType = entT, Message = message, NodeId = _nodeManagerHandler.NodeModel.Uuid };
            var jsonMsg = _singletonJsonHandler.JsonSerialize(msgModel);
            var sha256 = _singletonEncryptionHandler.SHA256(jsonMsg);
            msgModel.DataHash = sha256;
            msgModel.DataHashAlgo = "sha256";

            _logger.LogDebug("publish to exchange: '" + exchangeName + "', routingKey: '" + routingKey + "', message: '" + message + "', entityType: '" + entT + "'");
            Publish(exchangeName, msgModel, null, okAct, routingKey: routingKey);
        }

        public void Publish(string exchangeName, MessageModel msg, Func<MessageModel, MessageModelResponse> consumeAction, Action okAct, string routingKey = "")
        {
            if (Connection == null)
                return;
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
                                var messageProceedReturnModel = response;
                                if (messageProceedReturnModel is MessageOK)
                                {
                                    if (data.OkMsgAct != null)
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
