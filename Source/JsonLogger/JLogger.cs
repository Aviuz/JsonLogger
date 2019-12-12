﻿using JsonLogger.EventArguments;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace JsonLogger
{
    public class JLogger : IDisposable
    {
        #region Constants
        private const string EmptyArrayTemplate = "[\r\n]";
        private const string NextEntrySeperatorTemplate = ",\r\n";
        private const string CloseArrayTemplate = "\r\n]";

        private static readonly byte[] EmptyArray_Byte;
        private static readonly byte[] NextEntrySeperator_Byte;
        private static readonly byte[] CloseArray_Byte;

        private static int BufferSize = 128;
        #endregion

        #region Fields
        private bool isLogBufferEmpty => logBuffer.Length == 0;
        private StringBuilder logBuffer;
        #endregion

        #region Events
        public event EventHandler<AutomaticTransferingEvent> CustomAutomaticTransferFileName;
        #endregion

        #region Properties
        /// <summary>
        /// Path to log file where logger stores logs
        /// </summary>
        public string LogFilePath { get; private set; }

        /// <summary>
        /// Access file to get logs as JSON array
        /// </summary>
        public JArray LogJson
        {
            get
            {
                lock (FileLock)
                {
                    if (logBuffer.Length == 0)
                        return ReverseOrderOfArray(JArray.Parse(File.ReadAllText(LogFilePath)));

                    var logsInFile = JArray.Parse(File.ReadAllText(LogFilePath));
                    var logsInBuffer = JArray.Parse($"[{logBuffer.ToString()}]");

                    return ReverseOrderOfArray(new JArray(logsInFile.Union(logsInBuffer)));
                }
            }
        }

        /// <summary>
        /// Access file to get logs as plain text (formatted to JSON format)
        /// </summary>
        public string LogText => LogJson.ToString();

        /// <summary>
        /// Object to lock on when operating on log file. Locking this object will prevent all modifications on log file.
        /// </summary>
        public object FileLock => this;

        /// <summary>
        /// If set to true all changes are immadietely saved to log file.
        /// Otherwise changes are saved on SaveChanges() method or when logger is disposed.
        /// </summary>
        public bool AutoSave { get; set; }

        /// <summary>
        /// If set to value other than zero, it will automatically transfer file to backup file when source log file exceeds specified amout of bytes.
        /// </summary>
        public long TriggerAutomaticTransferSize { get; set; }
        #endregion

        #region Constructors
        static JLogger()
        {
            EmptyArray_Byte = Encoding.UTF8.GetBytes(EmptyArrayTemplate);
            NextEntrySeperator_Byte = Encoding.UTF8.GetBytes(NextEntrySeperatorTemplate);
            CloseArray_Byte = Encoding.UTF8.GetBytes(CloseArrayTemplate);
        }

        /// <summary>
        /// Creates new Json Logger with file path specified
        /// </summary>
        public JLogger(string filePath)
        {
            LogFilePath = filePath;

            logBuffer = new StringBuilder();
        }
        #endregion

        #region Logging Methods
        /// <summary>
        /// Logs single-line text with specified category
        /// </summary>
        public void Log(string text, LogCategory logCategory = LogCategory.Info)
        {
            var entry = new JObject();
            entry["title"] = text;
            entry["category"] = logCategory.ToString();
            entry["dataType"] = DataType.Text.ToString();
            entry["date"] = DateTime.Now.ToShortDateString();
            entry["time"] = DateTime.Now.TimeOfDay.ToString("hh\\:mm\\:ss");

            AppendLog(entry);
        }

        /// <summary>
        /// Logs block of text with specified category
        /// </summary>
        public void Log(string title, string text, LogCategory logCategory = LogCategory.Info)
        {
            var entry = new JObject();
            entry["title"] = title;
            entry["category"] = logCategory.ToString();
            entry["dataType"] = DataType.Text.ToString();
            entry["date"] = DateTime.Now.ToShortDateString();
            entry["time"] = DateTime.Now.TimeOfDay.ToString("hh\\:mm\\:ss");
            entry["data"] = text;

            AppendLog(entry);
        }

        /// <summary>
        /// Logs exception with it's properties as JSON object with default 'Critical' category
        /// </summary>
        public void Log(Exception e, string title = null, LogCategory logCategory = LogCategory.Critical)
        {
            var entry = new JObject();
            entry["title"] = title == null ? e.GetType().Name : title;
            entry["category"] = LogCategory.Critical.ToString();
            entry["dataType"] = DataType.Exception.ToString();
            entry["date"] = DateTime.Now.ToShortDateString();
            entry["time"] = DateTime.Now.TimeOfDay.ToString("hh\\:mm\\:ss");
            entry["data"] = JObject.FromObject(e);

            AppendLog(entry);
        }

        /// <summary>
        /// Logs objects with it's public properties and fields as JSON object
        /// </summary>
        /// <param name="title">Title for entry</param>
        /// <param name="item">Object that will be logged</param>
        /// <param name="logCategory">Category for entry</param>
        public void Log(object item, string title = null, LogCategory logCategory = LogCategory.Info)
        {
            var entry = new JObject();
            entry["title"] = title == null ? item.ToString() : title;
            entry["category"] = logCategory.ToString();
            entry["dataType"] = DataType.Object.ToString();
            entry["date"] = DateTime.Now.ToShortDateString();
            entry["time"] = DateTime.Now.TimeOfDay.ToString("hh\\:mm\\:ss");
            entry["data"] = FromObjectEx(item);

            AppendLog(entry);
        }

        /// <summary>
        /// Logs single-line text with 'Warning' category
        /// </summary>
        public void Warning(string text) => Log(text, LogCategory.Warning);

        /// <summary>
        /// Logs single-line text with 'Critical' category
        /// </summary>
        public void Error(string text) => Log(text, LogCategory.Critical);

        /// <summary>
        /// Logs exception with it's properties as JSON object (all exceptions are logged with 'Critical' category)
        /// </summary>
        public void Exception(string title, Exception e, LogCategory logCategory = LogCategory.Info) => Log(e);

        /// <summary>
        /// Logs objects with it's public properties and fields as JSON object
        /// </summary>
        /// <param name="title">Title for entry</param>
        /// <param name="item">Object that will be logged</param>
        /// <param name="logCategory">Category for entry</param>
        public void Object(string title, object item, LogCategory logCategory = LogCategory.Info) => Log(item, title, logCategory);
        #endregion

        /// <summary>
        /// Saves changes to log file (if AutoSave property is set to false, all uncommited changes are kept in memory)
        /// </summary>
        public void SaveChanges()
        {
            lock (FileLock)
            {
                var status = CheckFileQuick(LogFilePath);

                if (status == LogFileStatus.FileDoesntExists || status == LogFileStatus.FileEmpty)
                    InitializeFile(LogFilePath);
                else if (status == LogFileStatus.InvalidFormat)
                    throw new Exception($"Invalid format of log file for {nameof(JLogger)}");

                if (logBuffer.Length == 0)
                    return;

                byte[] buffer = Encoding.UTF8.GetBytes(logBuffer.ToString());

                using (var stream = new FileStream(LogFilePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    if (status == LogFileStatus.OnePlusEntry)
                    {
                        stream.Seek(-3, SeekOrigin.End);
                        stream.Write(NextEntrySeperator_Byte, 0, NextEntrySeperator_Byte.Length);
                    }
                    else
                    {
                        stream.Seek(-1, SeekOrigin.End);
                    }
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Write(CloseArray_Byte, 0, CloseArray_Byte.Length);
                    stream.Close();
                }

                logBuffer.Clear();
            }

            TryTransferLogFile();
        }

        /// <summary>
        /// Transfers logs from current file to another in a safe way.
        /// </summary>
        /// <param name="filePath">target file path</param>
        /// <param name="clearCurrentFile">true if current file should be purged</param>
        public void TranferLogToFile(string filePath, bool clearCurrentFile = true)
        {
            if (filePath == LogFilePath)
            {
                throw new ArgumentException();
            }

            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            lock (FileLock)
            {
                using (var input = new FileStream(LogFilePath, FileMode.Open, FileAccess.Read))
                using (var output = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = new byte[BufferSize];
                    int bytesToWrite;

                    do
                    {
                        bytesToWrite = input.Read(buffer, 0, BufferSize);
                        output.Write(buffer, 0, bytesToWrite);
                    }
                    while (bytesToWrite > 0);

                    input.Close();
                    output.Close();
                }

                InitializeFile(LogFilePath);
            }
        }

        #region Helpers Methods
        private static LogFileStatus CheckFileQuick(string path)
        {
            if (!File.Exists(path))
                return LogFileStatus.FileDoesntExists;

            char[] localBuffer = new char[4];
            using (var stream = File.OpenText(path))
            {
                stream.Read(localBuffer, 0, 4);
                stream.Close();
            }

            string text = new string(localBuffer);
            if (!text.StartsWith("[") || text.Length != 4)
            {
                return LogFileStatus.InvalidFormat;
            }
            else if (text == EmptyArrayTemplate)
            {
                return LogFileStatus.EmptyArray;
            }
            else
            {
                return LogFileStatus.OnePlusEntry;
            }
        }

        private static void InitializeFile(string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                stream.Write(EmptyArray_Byte, 0, EmptyArray_Byte.Length);
                stream.Close();
            }
        }

        private void AppendLog(JToken entry)
        {
            lock (logBuffer)
            {
                if (!isLogBufferEmpty)
                {
                    logBuffer.Append(NextEntrySeperatorTemplate);
                }

                logBuffer.Append(entry.ToString());
            }

            if (AutoSave)
            {
                SaveChanges();
            }
        }

        private static JArray ReverseOrderOfArray(JArray array)
        {
            var output = new JArray();
            for (int i = array.Count - 1; i >= 0; i--)
            {
                output.Add(array[i]);
            }
            return output;
        }

        private static JToken FromObjectEx(object item)
        {
            if (item == null)
            {
                return null;
            }
            else if(item.GetType().IsPrimitive || item is string)
            {
                return JValue.FromObject(item);
            }
            else if(item is IEnumerable)
            {
                return JArray.FromObject(item);
            }
            else
            {
                return JObject.FromObject(item);
            }
        }
        
        private void TryTransferLogFile()
        {
            if(TriggerAutomaticTransferSize > 0 && new FileInfo(LogFilePath).Length >= TriggerAutomaticTransferSize)
            {
                var args = new AutomaticTransferingEvent();
                var directory = Path.GetDirectoryName(LogFilePath);
                var fileName = string.Format("{0}.{1}.{2}", Path.GetFileNameWithoutExtension(LogFilePath), DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), Path.GetExtension(LogFilePath));
                args.FilePath = Path.Combine(directory, fileName);
                args.Cancel = File.Exists(args.FilePath);

                CustomAutomaticTransferFileName?.Invoke(this, args);

                if (!args.Cancel)
                {
                    TranferLogToFile(args.FilePath, true);
                }
            }
        }
        #endregion

        public void Dispose()
        {
            SaveChanges();
        }
    }
}
