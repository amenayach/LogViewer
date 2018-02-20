using System;

namespace LogViewer
{
    public class EntryInfo
    {
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Payload { get; set; }
        public string Url { get; set; }

        public override string ToString()
        {
            return $@"Type: {this.Type}
Date: {this.Date.ToShortDateString()}
Message: {this.Message}
StackTrace: {this.StackTrace}
Payload: {this.Payload}
Url: {this.Url}";
        }
    }
}
