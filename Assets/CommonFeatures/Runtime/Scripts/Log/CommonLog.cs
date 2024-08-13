using CommonFeatures.Config;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CommonFeatures.Log
{
    /// <summary>
    /// ��־����
    /// </summary>
    public class CommonLog
    {
        private static ELogType tags = 0;

        private static bool inited = false;

        /// <summary>
        /// ��־��¼����
        /// </summary>
        private const int LOG_RECORD_LENGTH = 500;

        /// <summary>
        /// ��־��¼
        /// </summary>
        public static List<LogRecord> LogRecord { get; private set; } = new List<LogRecord>(LOG_RECORD_LENGTH);

        /// <summary>
        /// ��ǰ��¼�±�,ָ����־��¼�ĵ�һ����¼
        /// </summary>
        public static int RecordCurIndex { get; private set; }

        private static void Init()
        {
            tags = ELogType.Config;//Ĭ�Ͽ���config����
            var config = CFM.Config.GetConfig<LogConfig>();
            //�������
            if (config.EnableNet)
            {
                tags |= ELogType.Net;
            }
            if (config.EnableTrace)
            {
                tags |= ELogType.Trace;
            }
            if (config.EnableResource)
            {
                tags |= ELogType.Resource;
            }
            if (config.EnableDebug)
            {
                tags |= ELogType.Debug;
            }

            LogRecord.Clear();
            RecordCurIndex = -1;
        }

        /// <summary>
        /// ��ӡĳ����ʱ������,��ʼ����,���<see cref="StopTimeWatch(System.Diagnostics.Stopwatch, string)"/>����ʹ��
        /// </summary>
        /// <param name="describe"></param>
        public static System.Diagnostics.Stopwatch StartTimeWatch(string describe = "Time")
        {
            UnityEngine.Debug.Log($"{describe} ��ʱ��ʼ");

            AddRecord("ffffff", $"{describe} ��ʱ��ʼ", LogType.Log);
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            return sw;
        }

        /// <summary>
        /// ��ӡĳ������ʱ������,��������,���<see cref="StartTimeWatch(string)"/>����ʹ��
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="describe"></param>
        public static void StopTimeWatch(System.Diagnostics.Stopwatch sw, string describe = "Time")
        {
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            UnityEngine.Debug.Log($"{describe} ��ʱ����, ʱ������: {ts.TotalMilliseconds}ms");

            AddRecord("ffffff", $"{describe} ��ʱ����, ʱ������: {ts.TotalMilliseconds}ms", LogType.Log);
        }

        /// <summary>
        /// ��ӡ�ֵ�
        /// </summary>
        public static void LogDictionary<T, K>(Dictionary<T, K> msg, Func<T, string> keyToString = null, Func<K, string> valueToString = null, string describe = null)
        {
            if (!IsOpen(ELogType.Trace))
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(describe))
            {
                sb.AppendLine(describe);
            }
            if (null != msg)
            {
                foreach (var pair in msg)
                {
                    sb.Append(keyToString?.Invoke(pair.Key) ?? (pair.Key?.ToString() ?? string.Empty));
                    sb.Append(" ");
                    sb.Append(valueToString?.Invoke(pair.Value) ?? (pair.Value?.ToString() ?? string.Empty));
                    sb.Append("\n");
                }
            }
            UnityEngine.Debug.Log(sb.ToString());

            AddRecord("ffffff", sb.ToString(), LogType.Log);
        }

        /// <summary>
        /// ��ӡ��׼��־
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        public static void Log(object msg, string color = "ffffff")
        {
            Print(ELogType.Trace, msg, LogType.Log, color);
        }

        /// <summary>
        /// �������־��ӡ
        /// </summary>
        /// <param name="msg"></param>
        public static void Net(object msg)
        {
            Print(ELogType.Net, msg, LogType.Log, "ee7700");
        }

        /// <summary>
        /// ������־��ӡ
        /// </summary>
        /// <param name="msg"></param>
        public static void Config(object msg)
        {
            Print(ELogType.Net, msg, LogType.Log, "708090");
        }

        /// <summary>
        /// ��Դ��־��ӡ
        /// </summary>
        /// <param name="msg"></param>
        public static void Resource(object msg)
        {
            Print(ELogType.Resource, msg, LogType.Log, "DAA520");
        }

        /// <summary>
        /// debug��־��ӡ
        /// </summary>
        /// <param name="msg"></param>
        public static void Debug(object msg)
        {
            Print(ELogType.Debug, msg, LogType.Log, "ADD8E6");
        }

        /// <summary>
        /// ��ӡ��׼����
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        public static void LogWarning(object msg, string color = "ffff00")
        {
            Print(ELogType.Trace, msg, LogType.Warning, color);
        }

        /// <summary>
        /// �������־����
        /// </summary>
        /// <param name="msg"></param>
        public static void NetWarning(object msg)
        {
            Print(ELogType.Net, msg, LogType.Warning, "ee7700");
        }

        /// <summary>
        /// ������־����
        /// </summary>
        /// <param name="msg"></param>
        public static void ConfigWarning(object msg)
        {
            Print(ELogType.Net, msg, LogType.Warning, "708090");
        }

        /// <summary>
        /// ��Դ��־����
        /// </summary>
        /// <param name="msg"></param>
        public static void ResourceWarning(object msg)
        {
            Print(ELogType.Resource, msg, LogType.Warning, "DAA520");
        }

        /// <summary>
        /// debug��־����
        /// </summary>
        /// <param name="msg"></param>
        public static void DebugWarning(object msg)
        {
            Print(ELogType.Debug, msg, LogType.Warning, "ADD8E6");
        }

        /// <summary>
        /// ��ӡ��׼����
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        public static void LogError(object msg, string color = "dc143c")
        {
            Print(ELogType.Trace, msg, LogType.Error, color);
        }

        /// <summary>
        /// �������־����
        /// </summary>
        /// <param name="msg"></param>
        public static void NetError(object msg)
        {
            Print(ELogType.Net, msg, LogType.Error, "ee7700");
        }

        /// <summary>
        /// ������־����
        /// </summary>
        /// <param name="msg"></param>
        public static void ConfigError(object msg)
        {
            Print(ELogType.Net, msg, LogType.Error, "708090");
        }

        /// <summary>
        /// ��Դ��־����
        /// </summary>
        /// <param name="msg"></param>
        public static void ResourceError(object msg)
        {
            Print(ELogType.Resource, msg, LogType.Error, "DAA520");
        }

        /// <summary>
        /// debug��־����
        /// </summary>
        /// <param name="msg"></param>
        public static void DebugError(object msg)
        {
            Print(ELogType.Debug, msg, LogType.Error, "ADD8E6");
        }

        /// <summary>
        /// ��ӡ����
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        private static void Print(ELogType logType, object msg, LogType type = LogType.Log, string color = "ffffff")
        {
            if (!IsOpen(logType))
            {
                return;
            }

            if (msg is string str)
            {
                switch (type)
                {
                    case LogType.Exception:
                    case LogType.Error:
                        UnityEngine.Debug.LogError($"<color=#{color}>{str}</color>");
                        break;
                    case LogType.Warning:
                        UnityEngine.Debug.LogWarning($"<color=#{color}>{str}</color>");
                        break;
                    case LogType.Log:
                    default:
                        UnityEngine.Debug.Log($"<color=#{color}>{str}</color>");
                        break;
                }

                AddRecord(color, str, type);
            }
            else if (msg is Exception ex)
            {
                switch (type)
                {
                    case LogType.Exception:
                    case LogType.Error:
                        UnityEngine.Debug.LogError($"<color=#{color}>{ex.Message}</color>\n<color=#{color}>{ex.StackTrace}</color>");
                        break;
                    case LogType.Warning:
                        UnityEngine.Debug.LogWarning($"<color=#{color}>{ex.Message}</color>\n<color=#{color}>{ex.StackTrace}</color>");
                        break;
                    case LogType.Log:
                    default:
                        UnityEngine.Debug.Log($"<color=#{color}>{ex.Message}</color>\n<color=#{color}>{ex.StackTrace}</color>");
                        break;
                }

                AddRecord(color, ex.Message, type);
            }
            else
            {
                switch (type)
                {
                    case LogType.Exception:
                    case LogType.Error:
                        UnityEngine.Debug.LogError($"<color=#{color}>{msg.ToString()}</color>");
                        break;
                    case LogType.Warning:
                        UnityEngine.Debug.LogWarning($"<color=#{color}>{msg.ToString()}</color>");
                        break;
                    case LogType.Log:
                    default:
                        UnityEngine.Debug.Log($"<color=#{color}>{msg.ToString()}</color>");
                        break;
                }

                AddRecord(color, msg.ToString(), type);
            }
        }

        private static bool IsOpen(ELogType tag)
        {
            if (!inited)
            {
                inited = true;
                Init();
            }
            return (tags & tag) != ELogType.Null;
        }

        /// <summary>
        /// ��Ӽ�¼
        /// </summary>
        /// <param name="color"></param>
        /// <param name="msg"></param>
        private static void AddRecord(string color, string msg, LogType logType)
        {
            if (RecordCurIndex < 0)
            {
                RecordCurIndex = 0;
                var record = new LogRecord();
                record.m_Color = color;
                record.m_Record = msg;
                record.m_LogType = logType;
                LogRecord.Add(record);
            }
            else
            {
                if (LogRecord.Count < LOG_RECORD_LENGTH)
                {
                    var record = new LogRecord();
                    record.m_Color = color;
                    record.m_Record = msg;
                    record.m_LogType = logType;
                    LogRecord.Add(record);
                }
                else
                {
                    var record = LogRecord[RecordCurIndex];
                    record.m_Color = color;
                    record.m_Record = msg;
                    record.m_LogType = logType;
                    RecordCurIndex++;
                    if (RecordCurIndex >= LOG_RECORD_LENGTH)
                    {
                        RecordCurIndex -= LOG_RECORD_LENGTH;
                    }
                }
            }
        }
    }

    /// <summary>
    /// ��־����
    /// </summary>
    public enum ELogType : byte
    {
        /// <summary>
        /// ����־
        /// </summary>
        Null,

        /// <summary>
        /// �������־
        /// </summary>
        Net = 1,

        /// <summary>
        /// ������־
        /// </summary>
        Config = 2,

        /// <summary>
        /// ��׼��־
        /// </summary>
        Trace = 4,

        /// <summary>
        /// ��Դ��־
        /// </summary>
        Resource = 8,

        /// <summary>
        /// Debug��־
        /// </summary>
        Debug = 16,
    }
}
