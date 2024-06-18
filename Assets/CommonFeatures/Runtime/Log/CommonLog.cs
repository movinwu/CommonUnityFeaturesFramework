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

        private static void Init()
        {
            tags = ELogType.Null;
            var config = CommonFeaturesManager.Config.GetConfig<LogConfig>();
            //�������
            if (config.EnableNet)
            {
                tags |= ELogType.Net;
            }
            if (config.EnableConfig)
            {
                tags |= ELogType.Config;
            }
            if (config.EnableTrace)
            {
                tags |= ELogType.Trace;
            }
            if (config.EnableResource)
            {
                tags |= ELogType.Resource;
            }
        }

        /// <summary>
        /// ������ʾ����־����,Ĭ��ֵ����ʵ�κ�������־
        /// </summary>
        /// <param name="tag"></param>
        //public static void SetTags(ELogType tag = ELogType.Null)
        //{
        //    tags = tag;
        //}

        /// <summary>
        /// ��ӡĳ����ʱ������,��ʼ����,���<see cref="StopTimeWatch(System.Diagnostics.Stopwatch, string)"/>����ʹ��
        /// </summary>
        /// <param name="describe"></param>
        public static System.Diagnostics.Stopwatch StartTimeWatch(string describe = "Time")
        {
            Debug.Log($"{describe} ��ʱ��ʼ");
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
            Debug.Log($"{describe} ��ʱ����, ʱ������: {ts.TotalMilliseconds}ms");
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
            Debug.Log(sb.ToString());
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
        /// ��ӡ��׼�쳣
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        public static void TraceException(object msg, string color = "ffffff")
        {
            Print(ELogType.Trace, msg, LogType.Exception, color);
        }

        /// <summary>
        /// �������־�쳣
        /// </summary>
        /// <param name="msg"></param>
        public static void NetException(object msg)
        {
            Print(ELogType.Net, msg, LogType.Exception, "ee7700");
        }

        /// <summary>
        /// ���ݲ���־�쳣
        /// </summary>
        /// <param name="msg"></param>
        public static void ModelException(object msg)
        {
            Print(ELogType.Net, msg, LogType.Exception, "9370DB");
        }

        /// <summary>
        /// ҵ�����־�쳣
        /// </summary>
        /// <param name="msg"></param>
        public static void BusinessException(object msg)
        {
            Print(ELogType.Net, msg, LogType.Exception, "0000cd");
        }

        /// <summary>
        /// ��ͼ����־�쳣
        /// </summary>
        /// <param name="msg"></param>
        public static void ViewException(object msg)
        {
            Print(ELogType.Net, msg, LogType.Exception, "20B2AA");
        }

        /// <summary>
        /// ������־�쳣
        /// </summary>
        /// <param name="msg"></param>
        public static void ConfigException(object msg)
        {
            Print(ELogType.Net, msg, LogType.Exception, "708090");
        }

        /// <summary>
        /// ��Դ��־�쳣
        /// </summary>
        /// <param name="msg"></param>
        public static void ResourceException(object msg)
        {
            Print(ELogType.Resource, msg, LogType.Exception, "DAA520");
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
                        Debug.LogError($"<color=#{color}>{str}</color>");
                        break;
                    case LogType.Warning:
                        Debug.LogWarning($"<color=#{color}>{str}</color>");
                        break;
                    case LogType.Log:
                    default:
                        Debug.Log($"<color=#{color}>{str}</color>");
                        break;
                }
            }
            else if (msg is Exception ex)
            {
                switch (type)
                {
                    case LogType.Exception:
                    case LogType.Error:
                        Debug.LogError($"<color=#{color}>{ex.Message}</color>\n<color=#{color}>{ex.StackTrace}</color>");
                        break;
                    case LogType.Warning:
                        Debug.LogWarning($"<color=#{color}>{ex.Message}</color>\n<color=#{color}>{ex.StackTrace}</color>");
                        break;
                    case LogType.Log:
                    default:
                        Debug.Log($"<color=#{color}>{ex.Message}</color>\n<color=#{color}>{ex.StackTrace}</color>");
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case LogType.Exception:
                    case LogType.Error:
                        Debug.LogError($"<color=#{color}>{msg.ToString()}</color>");
                        break;
                    case LogType.Warning:
                        Debug.LogWarning($"<color=#{color}>{msg.ToString()}</color>");
                        break;
                    case LogType.Log:
                    default:
                        Debug.Log($"<color=#{color}>{msg.ToString()}</color>");
                        break;
                }
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
    }
}
