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
    public class Logger
    {
        public IDisposable StartWriteThread()
        {
            if (_writeThread is not null)
                throw new InvalidProgramException("");

            _writeThread = new Timer(_ =>
            {
                WriteSoon();
            }, this, 3000, 3000);
            return _writeThread;
        }

        public void WriteSoon()
            => WriteLogCore();

        public void PushLog(LogObject inLog)
        {
            lock (_writeQueue)
            {
                _writeQueue.Enqueue(inLog);
            }
        }

        public void PushLog(Exception inException)
            => PushLog(new LogObject().SetException(inException));

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
        private bool _isWriting = false;
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

        public class ExceptionLog
        {
            public ExceptionLog(Exception inException)
            {
                Type = inException.GetType().Name;
                Message = inException.Message;
                StackTrace = inException.StackTrace ?? "";
            }

            [JsonProperty("type")]
            public string Type
            { get; set; } = string.Empty;

            [JsonProperty("message")]
            public string Message
            { get; set; } = string.Empty;

            [JsonProperty("stacktrace")]
            public string StackTrace
            { get; set; } = string.Empty;
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

            public LogObject SetException(Exception inException)
            {
                Level = LogLevel.Exception;
                Message = null;
                Exception = new ExceptionLog(inException);

                return this;
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

            [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
            public string? Message
            { get; set; } = null;

            [JsonProperty("exception", NullValueHandling = NullValueHandling.Ignore)]
            public ExceptionLog? Exception
            { get; set; } = null;
        }
    }
}
