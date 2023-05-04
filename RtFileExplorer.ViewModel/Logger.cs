using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RtFileExplorer.ViewModel.Wpf.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

namespace RtFileExplorer.ViewModel.Wpf
{
    internal class Logger
    {
        public IDisposable StartWriteThread()
        {
            if (_writeThread is not null)
                throw new InvalidProgramException("");

            _writeThread = new Timer(_ =>
            {
                WriteLogCore();
            }, this, 3000, 3000);
            return _writeThread;
        }

        public void PushLog(LogObject inLog)
        {
            lock (_writeQueue)
            {
                _writeQueue.Enqueue(inLog);
            }
        }

        private void WriteLogCore()
        {
            lock (_writeQueue)
            {
                if (_writeQueue.Count == 0)
                    return;

                var serializerSettings = new JsonSerializerSettings
                {
                    Converters = {
                        new IsoDateTimeConverter { DateTimeFormat = "HH:mm:ss" }
                    }
                };

                var filepath = $"{ApplicationViewModel.ApplicationDataDirectoryPath}/Log/{DateTime.Now.ToString("yyyyMMdd")}.log";
                var directory = Path.GetDirectoryName(filepath)!;
                if (!System.IO.Directory.Exists(directory))
                    System.IO.Directory.CreateDirectory(directory);

                using (var writer = new StreamWriter(new FileStream(filepath, FileMode.Append, FileAccess.Write)))
                {
                    while (_writeQueue.Count > 0)
                    {
                        var obj = _writeQueue.Dequeue();

                        var line = JsonConvert.SerializeObject(obj, serializerSettings);
                        writer.WriteLine(line);
                    }
                }
            }
        }

        public static Logger Instance { get; } = new Logger();
        private Timer? _writeThread = null;
        private readonly Queue<LogObject> _writeQueue = new Queue<LogObject>();

        public enum LogLevel
        {
            [EnumMember(Value = "information")]
            Information,
            [EnumMember(Value = "warning")]
            Warning,
            [EnumMember(Value = "error")]
            Error,
            [EnumMember(Value = "exception")]
            Exception
        }

        public class LogObject
        {
            public LogObject(
                [CallerFilePath] string inFilePath = "",
                [CallerLineNumber] int inLineNumber = 0,
                [CallerMemberName] string inMemberName = "")
            {
                Filepath = inFilePath;
                LineNumber = inLineNumber;
                CallerMemberName = inMemberName;
            }

            [JsonProperty("timestamp")]
            public DateTime Timestamp
            { get; set; } = DateTime.Now;

            [JsonProperty("filepath")]
            public string Filepath
            { get; set; } = string.Empty;

            [JsonProperty("line")]
            public int LineNumber
            { get; set; }

            [JsonProperty("caller")]
            public string CallerMemberName
            { get; set; } = string.Empty;

            [JsonProperty("level"), JsonConverter(typeof(StringEnumConverter))]
            public LogLevel Level
            { get; set; }

            [JsonProperty("message")]
            public string Message
            { get; set; } = string.Empty;
        }
    }
}
