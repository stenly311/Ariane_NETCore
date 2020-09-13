namespace NLog.RabbitMQ.Appender
{
    public static class Configuration
    {
        public const string AppenderName = "RabbitMQ";

        public static class RabbitMQ
        {
            public static string ExhangeName = "Ariane.MQ";
            public static bool Autodelete = true;
        }
    }
}
