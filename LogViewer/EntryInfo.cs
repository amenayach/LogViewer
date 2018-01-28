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
    }
}
