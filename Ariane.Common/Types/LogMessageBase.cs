using System;

namespace Ariane.Common.Types
{
    public class LogMessageBase
    {
        public DateTime TimeStamp { get; set; }
        public string LoggerName { get; set; }
        public string FormattedMessage { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
    }
}
