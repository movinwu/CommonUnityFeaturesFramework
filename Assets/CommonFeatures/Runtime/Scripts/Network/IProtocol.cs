using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// ͨ�Žӿ�
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// ͨ��id
        /// </summary>
        short MsgId { get; }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="data"></param>
        void ReceiveMessage(byte[] data);

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="data"></param>
        void SendMessage(byte[] data);
    }
}