using CommonFeatures.Config;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CommonFeatures.Log
{
    /// <summary>
    /// 日志管理
    /// </summary>
    public class CommonLog
    {
        private static ELogType tags = 0;

        private static bool inited = false;

        private static void Init()
        {
            //tags = ELogType.Net
            //    | ELogType.Model
            //    | ELogType.Business
            //    | ELogType.View
            //    | ELogType.Config
            //    | ELogType.Trace;

            tags = ELogType.Null;
            //检查配置
            if (CFM.Config.GetBooleanConfig("Log", "net"))
            {
                tags |= ELogType.Net;
            }
            if (CFM.Config.GetBooleanConfig("Log", "model"))
            {
                tags |= ELogType.Model;
            }
            if (CFM.Config.GetBooleanConfig("Log", "business"))
            {
                tags |= ELogType.Business;
            }
            if (CFM.Config.GetBooleanConfig("Log", "view"))
            {
                tags |= ELogType.View;
            }
            if (CFM.Config.GetBooleanConfig("Log", "config"))
            {
                tags |= ELogType.Config;
            }
            if (CFM.Config.GetBooleanConfig("Log", "trace"))
            {
                tags |= ELogType.Trace;
            }
            if (CFM.Config.GetBooleanConfig("Log", "resource"))
            {
                tags |= ELogType.Resource;
            }
        }

        /// <summary>
        /// 设置显示的日志类型,默认值不现实任何类型日志
        /// </summary>
        /// <param name="tag"></param>
        //public static void SetTags(ELogType tag = ELogType.Null)
        //{
        //    tags = tag;
        //}

        /// <summary>
        /// 打印某操作时间消耗,开始监视,配合<see cref="StopTimeWatch(System.Diagnostics.Stopwatch, string)"/>函数使用
        /// </summary>
        /// <param name="describe"></param>
        public static System.Diagnostics.Stopwatch StartTimeWatch(string describe = "Time")
        {
            Debug.Log($"{describe} 计时开始");
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            return sw;
        }

        /// <summary>
        /// 打印某操作的时间消耗,结束监视,配合<see cref="StartTimeWatch(string)"/>函数使用
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="describe"></param>
        public static void StopTimeWatch(System.Diagnostics.Stopwatch sw, string describe = "Time")
        {
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Debug.Log($"{describe} 计时结束, 时间消耗: {ts.TotalMilliseconds}ms");
        }

        /// <summary>
        /// 打印字典
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
        /// 打印标准日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        public static void Log(object msg, string color = "ffffff")
        {
            Print(ELogType.Trace, msg, LogType.Log, color);
        }

        /// <summary>
        /// 网络层日志打印
        /// </summary>
        /// <param name="msg"></param>
        public static void Net(object msg)
        {
            Print(ELogType.Net, msg, LogType.Log, "ee7700");
        }

        /// <summary>
        /// 数据层日志打印
        /// </summary>
        /// <param name="msg"></param>
        public static void Model(object msg)
        {
            Print(ELogType.Net, msg, LogType.Log, "9370DB");
        }

        /// <summary>
        /// 业务层日志打印
        /// </summary>
        /// <param name="msg"></param>
        public static void Business(object msg)
        {
            Print(ELogType.Net, msg, LogType.Log, "0000cd");
        }

        /// <summary>
        /// 视图层日志打印
        /// </summary>
        /// <param name="msg"></param>
        public static void View(object msg)
        {
            Print(ELogType.Net, msg, LogType.Log, "20B2AA");
        }

        /// <summary>
        /// 配置日志打印
        /// </summary>
        /// <param name="msg"></param>
        public static void Config(object msg)
        {
            Print(ELogType.Net, msg, LogType.Log, "708090");
        }

        /// <summary>
        /// 资源日志打印
        /// </summary>
        /// <param name="msg"></param>
        public static void Resource(object msg)
        {
            Print(ELogType.Resource, msg, LogType.Log, "DAA520");
        }

        /// <summary>
        /// 打印标准警告
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        public static void LogWarning(object msg, string color = "ffff00")
        {
            Print(ELogType.Trace, msg, LogType.Warning, color);
        }

        /// <summary>
        /// 网络层日志警告
        /// </summary>
        /// <param name="msg"></param>
        public static void NetWarning(object msg)
        {
            Print(ELogType.Net, msg, LogType.Warning, "ee7700");
        }

        /// <summary>
        /// 数据层日志警告
        /// </summary>
        /// <param name="msg"></param>
        public static void ModelWarning(object msg)
        {
            Print(ELogType.Net, msg, LogType.Warning, "9370DB");
        }

        /// <summary>
        /// 业务层日志警告
        /// </summary>
        /// <param name="msg"></param>
        public static void BusinessWarning(object msg)
        {
            Print(ELogType.Net, msg, LogType.Warning, "0000cd");
        }

        /// <summary>
        /// 视图层日志警告
        /// </summary>
        /// <param name="msg"></param>
        public static void ViewWarning(object msg)
        {
            Print(ELogType.Net, msg, LogType.Warning, "20B2AA");
        }

        /// <summary>
        /// 配置日志警告
        /// </summary>
        /// <param name="msg"></param>
        public static void ConfigWarning(object msg)
        {
            Print(ELogType.Net, msg, LogType.Warning, "708090");
        }

        /// <summary>
        /// 资源日志警告
        /// </summary>
        /// <param name="msg"></param>
        public static void ResourceWarning(object msg)
        {
            Print(ELogType.Resource, msg, LogType.Warning, "DAA520");
        }

        /// <summary>
        /// 打印标准错误
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        public static void LogError(object msg, string color = "dc143c")
        {
            Print(ELogType.Trace, msg, LogType.Error, color);
        }

        /// <summary>
        /// 网络层日志错误
        /// </summary>
        /// <param name="msg"></param>
        public static void NetError(object msg)
        {
            Print(ELogType.Net, msg, LogType.Error, "ee7700");
        }

        /// <summary>
        /// 数据层日志错误
        /// </summary>
        /// <param name="msg"></param>
        public static void ModelError(object msg)
        {
            Print(ELogType.Net, msg, LogType.Error, "9370DB");
        }

        /// <summary>
        /// 业务层日志错误
        /// </summary>
        /// <param name="msg"></param>
        public static void BusinessError(object msg)
        {
            Print(ELogType.Net, msg, LogType.Error, "0000cd");
        }

        /// <summary>
        /// 视图层日志错误
        /// </summary>
        /// <param name="msg"></param>
        public static void ViewError(object msg)
        {
            Print(ELogType.Net, msg, LogType.Error, "20B2AA");
        }

        /// <summary>
        /// 配置日志错误
        /// </summary>
        /// <param name="msg"></param>
        public static void ConfigError(object msg)
        {
            Print(ELogType.Net, msg, LogType.Error, "708090");
        }

        /// <summary>
        /// 资源日志错误
        /// </summary>
        /// <param name="msg"></param>
        public static void ResourceError(object msg)
        {
            Print(ELogType.Resource, msg, LogType.Error, "DAA520");
        }

        /// <summary>
        /// 打印标准异常
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        public static void TraceException(object msg, string color = "ffffff")
        {
            Print(ELogType.Trace, msg, LogType.Exception, color);
        }

        /// <summary>
        /// 网络层日志异常
        /// </summary>
        /// <param name="msg"></param>
        public static void NetException(object msg)
        {
            Print(ELogType.Net, msg, LogType.Exception, "ee7700");
        }

        /// <summary>
        /// 数据层日志异常
        /// </summary>
        /// <param name="msg"></param>
        public static void ModelException(object msg)
        {
            Print(ELogType.Net, msg, LogType.Exception, "9370DB");
        }

        /// <summary>
        /// 业务层日志异常
        /// </summary>
        /// <param name="msg"></param>
        public static void BusinessException(object msg)
        {
            Print(ELogType.Net, msg, LogType.Exception, "0000cd");
        }

        /// <summary>
        /// 视图层日志异常
        /// </summary>
        /// <param name="msg"></param>
        public static void ViewException(object msg)
        {
            Print(ELogType.Net, msg, LogType.Exception, "20B2AA");
        }

        /// <summary>
        /// 配置日志异常
        /// </summary>
        /// <param name="msg"></param>
        public static void ConfigException(object msg)
        {
            Print(ELogType.Net, msg, LogType.Exception, "708090");
        }

        /// <summary>
        /// 资源日志异常
        /// </summary>
        /// <param name="msg"></param>
        public static void ResourceException(object msg)
        {
            Print(ELogType.Resource, msg, LogType.Exception, "DAA520");
        }

        /// <summary>
        /// 打印函数
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
    /// 日志类型
    /// </summary>
    public enum ELogType : byte
    {
        /// <summary>
        /// 无日志
        /// </summary>
        Null,

        /// <summary>
        /// 网络层日志
        /// </summary>
        Net = 1,

        /// <summary>
        /// 数据结构层日志
        /// </summary>
        Model = 2,

        /// <summary>
        /// 业务逻辑层日志
        /// </summary>
        Business = 4,

        /// <summary>
        /// 视图层日志
        /// </summary>
        View = 8,

        /// <summary>
        /// 配置日志
        /// </summary>
        Config = 16,

        /// <summary>
        /// 标准日志
        /// </summary>
        Trace = 32,

        /// <summary>
        /// 资源日志
        /// </summary>
        Resource = 64,
    }
}
