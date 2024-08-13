using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Log
{

    /// <summary>
    /// 日志记录
    /// </summary>
    public class LogRecord
    {
        /// <summary>
        /// 记录
        /// </summary>
        public string m_Record;

        /// <summary>
        /// 日志颜色
        /// </summary>
        public string m_Color;

        /// <summary>
        /// 日志类型
        /// </summary>
        public UnityEngine.LogType m_LogType;

        public LogRecord Clone()
        {
            var record = new LogRecord();
            record.m_Record = m_Record;
            record.m_Color = m_Color;
            record.m_LogType = m_LogType;
            return record;
        }
    }
}
