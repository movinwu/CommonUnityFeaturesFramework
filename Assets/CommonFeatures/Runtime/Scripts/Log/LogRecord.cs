using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Log
{

    /// <summary>
    /// ��־��¼
    /// </summary>
    public class LogRecord
    {
        /// <summary>
        /// ��¼
        /// </summary>
        public string m_Record;

        /// <summary>
        /// ��־��ɫ
        /// </summary>
        public string m_Color;

        /// <summary>
        /// ��־����
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
