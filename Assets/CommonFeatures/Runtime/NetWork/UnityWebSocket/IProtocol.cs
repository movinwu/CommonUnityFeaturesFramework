using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// 通信接口
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// 通信id
        /// </summary>
        short MsgId { get; }

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="data"></param>
        void ReceiveMessage(byte[] data);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data"></param>
        void SendMessage(byte[] data);
    }
}
