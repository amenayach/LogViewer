using System;
using System.Collections.Generic;
using System.IO;

namespace LogViewer
{
    public class LogReader
    {
        public static IEnumerable<EntryInfo> LoadFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                var lines = File.ReadAllLines(fileName);

                var records = new List<EntryInfo>();

                EntryInfo record = null;
                string latestProperty = "";
                string latestPropertyValue = "";

                foreach (var line in lines)
                {
                    if (line.ToLower().StartsWith(Constants.Splitter.ToLower()))
                    {
                        SetValueIfNeeded(ref latestProperty, record, ref latestPropertyValue);
                        record = new EntryInfo();
                        records.Add(record);
                    }
                    else if (line.ToLower().StartsWith("type") ||
                               line.ToLower().StartsWith("url"))
                    {
                        SetValueIfNeeded(ref latestProperty, record, ref latestPropertyValue);
                        var propertyValue = line.Substring(line.IndexOf(':') + 1);
                        Extensions.SetPropertyValue(record, line.Substring(0, line.IndexOf(':')), propertyValue);
                    }
                    else if (line.ToLower().StartsWith("date"))
                    {
                        SetValueIfNeeded(ref latestProperty, record, ref latestPropertyValue);
                        var propertyValue = DateTime.Parse(line.Substring(line.IndexOf(':') + 1));
                        Extensions.SetPropertyValue(record, line.Substring(0, line.IndexOf(':')), propertyValue);
                    }
                    else if (line.ToLower().StartsWith("message"))
                    {
                        if (latestProperty.ToLower() != "message") SetValueIfNeeded(ref latestProperty, record, ref latestPropertyValue);
                        latestPropertyValue = line.Substring(line.IndexOf(':') + 1);
                        latestProperty = line.Substring(0, line.IndexOf(':'));
                    }
                    else if (line.ToLower().StartsWith("payload"))
                    {
                        if (latestProperty.ToLower() != "payload") SetValueIfNeeded(ref latestProperty, record, ref latestPropertyValue);
                        latestPropertyValue = line.Substring(line.IndexOf(':') + 1);
                        latestProperty = line.Substring(0, line.IndexOf(':'));
                    }
                    else if (line.ToLower().StartsWith("stacktrace"))
                    {
                        if (latestProperty.ToLower() != "stacktrace") SetValueIfNeeded(ref latestProperty, record, ref latestPropertyValue);
                        latestPropertyValue = line.Substring(line.IndexOf(':') + 1);
                        latestProperty = line.Substring(0, line.IndexOf(':'));
                    }
                    else if (!string.IsNullOrWhiteSpace(line))
                    {
                        latestPropertyValue += Environment.NewLine + line;
                    }
                }

                SetValueIfNeeded(ref latestProperty, record, ref latestPropertyValue);

                return records;
            }

            return null;
        }

        private static void SetValueIfNeeded(ref string latestProperty, EntryInfo record, ref string latestPropertyValue)
        {
            if (!string.IsNullOrWhiteSpace(latestProperty) && !string.IsNullOrWhiteSpace(latestProperty))
            {
                Extensions.SetPropertyValue(record, latestProperty, latestPropertyValue);
                latestProperty = string.Empty;
                latestPropertyValue = string.Empty;
            }
        }

    }
}
