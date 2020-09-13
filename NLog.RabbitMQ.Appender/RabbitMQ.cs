using Ariane.Common;
using Ariane.Common.Types;
using EasyNetQ;
using EasyNetQ.Topology;
using NLog.Targets;
using System;
using System.Configuration;

namespace NLog.RabbitMQ.Appender
{
    [Target(Configuration.AppenderName)]
    public class RabbitMQ : TargetWithLayout
    {
        IBus bus;
        IExchange exchange;
        public RabbitMQ()
        {
            var conn = ConfigurationManager.AppSettings.Get(Constants.RabbitMQConnectionName);
            if (conn != null)
            {
                bus = RabbitHutch.CreateBus(conn);
                exchange = bus.Advanced.ExchangeDeclare(Configuration.RabbitMQ.ExhangeName, ExchangeType.Topic, autoDelete: Configuration.RabbitMQ.Autodelete);
            }
            else
            {
                Console.WriteLine($"No RabbitMQ connection string found in application web.confg file. Please, make sure that field" +
                    $"'{Constants.RabbitMQConnectionName}' exists in appConfig section and has a connection string as a value.");
            }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if (bus!= null && bus.IsConnected)
            {
                bus.Advanced.Publish(exchange, logEvent.LoggerName, true, new Message<LogMessageBase>(new LogMessageBase
                {
                    TimeStamp = logEvent.TimeStamp,
                    Level = logEvent.Level.ToString(),
                    FormattedMessage = logEvent.FormattedMessage,
                    Message = logEvent.Message,
                    LoggerName = logEvent.LoggerName
                }));
            }
            else
                Console.WriteLine("Not connected.");
        }

        protected override void CloseTarget()
        {
            bus?.Dispose();
            base.CloseTarget();
        }
    }
}
