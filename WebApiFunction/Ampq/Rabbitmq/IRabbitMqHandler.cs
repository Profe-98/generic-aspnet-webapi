using System;
using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client.Logging;
using WebApiFunction.Ampq.Rabbitmq.Data;

namespace WebApiFunction.Ampq.Rabbitmq
{
    public interface IRabbitMqHandler
    {
        public IConnection Connection { get; }
        public IConnection GetConnection(EventHandler<ConnectionBlockedEventArgs> connectionBlockedEventHandler = null,
            EventHandler<CallbackExceptionEventArgs> callBackExceptionEventHandler = null,
            EventHandler<EventArgs> connectionUnblockedEventHandler = null,
            EventHandler<ShutdownEventArgs> shutDownEventHandler = null);

        public void PublishObject(string exchangeName, object entity, string routingKey, string message);
        public void Publish(string exchangeName, MessageModel msg, Func<MessageModel, MessageModelResponse> consumeAction = null, string routingKey = "");
        public void SubscibeExchange(string exchangeName, Func<MessageModel, MessageModelResponse> consumeAction, string routingKey = "");


    }
}
