using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OOPS
{
    /// <summary>
    /// 通信管理器,管理协议key和类型
    /// </summary>
    public class ProtocolManager : SingletonBase<ProtocolManager>
    {
        private ProtocolManager() { }

        /// <summary>
        /// 所有协议生成函数
        /// </summary>
        private Dictionary<short, System.Func<IProtocol>> m_ProtocolGenerateFuncDic = new Dictionary<short, System.Func<IProtocol>>();

        /// <summary>
        /// 所有协议类型
        /// </summary>
        private Dictionary<System.Type, short> m_ProtocolTypeDic = new Dictionary<System.Type, short>();

        /// <summary>
        /// 注册协议生成函数
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="generateFunc"></param>
        /// <param name="protocolType"></param>
        public void RegisterProtocolGenerate(short msgId, System.Func<IProtocol> generateFunc, System.Type protocolType)
        {
            if (m_ProtocolTypeDic.ContainsKey(protocolType))
            {
                Logger.NetError($"协议生成器重复注册, 类型: {protocolType}, id: {msgId}");
            }
            else if (m_ProtocolGenerateFuncDic.ContainsKey(msgId))
            {
                Logger.NetError($"协议生成器重复注册, 类型: {protocolType}, id: {msgId}");
            }
            else if (!typeof(Google.Protobuf.IMessage).IsAssignableFrom(protocolType))
            {
                Logger.NetError($"协议生成器注册失败,类型{protocolType}必须是protobuf数据类型");
            }
            else
            {
                m_ProtocolTypeDic.Add(protocolType, msgId);
                m_ProtocolGenerateFuncDic.Add(msgId, generateFunc);
            }
        }

        /// <summary>
        /// 根据id生成协议
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public IProtocol GenerateProtocol(short msgId)
        {
            if (m_ProtocolGenerateFuncDic.TryGetValue(msgId, out var func))
            {
                return func?.Invoke();
            }
            Logger.NetError($"协议生成器没有注册, id: {msgId}");
            return null;
        }

        /// <summary>
        /// 获取id
        /// </summary>
        /// <param name="protocolType"></param>
        /// <returns></returns>
        public short GetProtocolId(System.Type protocolType)
        {
            if (m_ProtocolTypeDic.TryGetValue(protocolType, out var id))
            {
                return id;
            }
            else
            {
                Logger.NetError($"协议生成器没有注册, 类型: {protocolType}");
                return -1;
            }
        }
    }
}
