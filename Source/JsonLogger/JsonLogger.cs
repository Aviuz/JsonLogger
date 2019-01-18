﻿using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;

namespace JsonLogger
{
    public class JsonLogger : IDisposable
    {
        #region Constants
        private const string EmptyArrayTemplate = "[\r\n]";
        private const string NextEntrySeperatorTemplate = ",\r\n";
        private const string CloseArrayTemplate = "\r\n]";

        private static readonly byte[] EmptyArray_Byte;
        private static readonly byte[] NextEntrySeperator_Byte;
        private static readonly byte[] CloseArray_Byte;
        #endregion

        #region Private Fields
        private bool isLogBufferEmpty => logBuffer.Length == 0;
        private StringBuilder logBuffer;
        #endregion

        #region Public Properties
        /// <summary>
        /// Path to log file where logger stores logs
        /// </summary>
        public string LogFilePath { get; private set; }

        /// <summary>
        /// Access file to get logs as JSON array
        /// </summary>
        public JArray LogJson => ReverseOrderOfArray(JArray.Parse(File.ReadAllText(LogFilePath)));

        /// <summary>
        /// Access file to get logs as plain text (formatted to JSON format)
        /// </summary>
        public string LogText => LogJson.ToString();
        #endregion
        
        /// <summary>
        /// If set to true all changes are immadietely saved to log file.
        /// Otherwise changes are saved on SaveChanges() method or when logger is disposed.
        /// </summary>
        public bool AutoSave { get; set; }
        #region Constructors
        static JsonLogger()
        {
            EmptyArray_Byte = Encoding.UTF8.GetBytes(EmptyArrayTemplate);
            NextEntrySeperator_Byte = Encoding.UTF8.GetBytes(NextEntrySeperatorTemplate);
            CloseArray_Byte = Encoding.UTF8.GetBytes(CloseArrayTemplate);
        }

        public JsonLogger(string filePath)
        {
            LogFilePath = filePath;

            logBuffer = new StringBuilder();
        }
        #endregion

        #region Logging Methods
        /// <summary>
        /// Logs single-line text with specified category
        /// </summary>
        public void Log(string text, LogCategory logCategory)
        {
            var entry = new JObject();
            entry["title"] = text;
            entry["category"] = logCategory.ToString();
            entry["dataType"] = DataType.Text.ToString();
            entry["date"] = DateTime.Now.ToShortDateString();
            entry["time"] = DateTime.Now.TimeOfDay.ToString();

            AppendLog(entry);
        }

        /// <summary>
        /// Logs exception with it's properties as JSON object (all exceptions are logged with 'Critical' category)
        /// </summary>
        public void Log(Exception e)
        {
            var entry = new JObject();
            entry["title"] = e.GetType().Name;
            entry["category"] = LogCategory.Critical.ToString();
            entry["dataType"] = DataType.Exception.ToString();
            entry["date"] = DateTime.Now.ToShortDateString();
            entry["time"] = DateTime.Now.TimeOfDay.ToString();
            entry["data"] = JObject.FromObject(e);

            AppendLog(entry);
        }

        /// <summary>
        /// Logs objects with it's public properties and fields as JSON object
        /// </summary>
        /// <param name="title">Title for entry</param>
        /// <param name="item">Object that will be logged</param>
        /// <param name="logCategory">Category for entry</param>
        public void Log(string title, object item, LogCategory logCategory)
        {
            var entry = new JObject();
            entry["title"] = title;
            entry["category"] = LogCategory.Critical.ToString();
            entry["dataType"] = DataType.Exception.ToString();
            entry["date"] = DateTime.Now.ToShortDateString();
            entry["time"] = DateTime.Now.TimeOfDay.ToString();
            entry["data"] = JObject.FromObject(item);

            AppendLog(entry);
        }

        /// <summary>
        /// Logs single-line text with 'Info' category
        /// </summary>
        public void LogMessage(string text) => Log(text, LogCategory.Info);

        /// <summary>
        /// Logs single-line text with 'Warning' category
        /// </summary>
        public void LogWarning(string text) => Log(text, LogCategory.Warning);

        /// <summary>
        /// Logs single-line text with 'Critical' category
        /// </summary>
        public void LogError(string text) => Log(text, LogCategory.Critical);

        /// <summary>
        /// Logs exception with it's properties as JSON object (all exceptions are logged with 'Critical' category)
        /// </summary>
        public void LogException(Exception e) => Log(e);

        /// <summary>
        /// Logs objects with it's public properties and fields as JSON object
        /// </summary>
        /// <param name="title">Title for entry</param>
        /// <param name="item">Object that will be logged</param>
        /// <param name="logCategory">Category for entry</param>
        public void LogObject(string title, object item, LogCategory logCategory) => Log(title, item, logCategory);
        #endregion

        /// <summary>
        /// Saves changes to log file (if AutoSave property is set to false, all uncommited changes are kept in memory)
        /// </summary>
        public void SaveChanges()
        {
            var status = CheckFileQuick(LogFilePath);

            if (status == LogFileStatus.FileDoesntExists || status == LogFileStatus.FileEmpty)
                InitializeFile(LogFilePath);
            else if (status == LogFileStatus.InvalidFormat)
                throw new Exception($"Invalid format of log file for {nameof(JsonLogger)}");

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
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                stream.Write(EmptyArray_Byte, 0, EmptyArray_Byte.Length);
                stream.Close();
            }
        }

        private void AppendLog(JToken entry)
        {
            if (!isLogBufferEmpty)
                logBuffer.Append(NextEntrySeperatorTemplate);

            logBuffer.Append(entry.ToString());

            if (AutoSave)
                SaveChanges();
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
        #endregion

        public void Dispose()
        {
            SaveChanges();
        }
    }
}
