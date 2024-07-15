using CommonFeatures.Log;
using CommonFeatures.Pool;
using CommonFeatures.Singleton;
using System.Collections.Generic;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// 通信管理器,管理协议key和类型
    /// </summary>
    public class ProtocolManager : SingletonBase<ProtocolManager>
    {
        private ProtocolManager() { }

        /// <summary>
        /// 所有协议类型
        /// </summary>
        private Dictionary<short, System.Type> m_ProtocolTypeDic = new Dictionary<short, System.Type>();

        /// <summary>
        /// 注册协议生成函数
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="protocolType"></param>
        public void RegisterProtocol(short msgId, System.Type protocolType)
        {
            if (m_ProtocolTypeDic.ContainsKey(msgId))
            {
                CommonLog.NetError($"协议生成器重复注册, 类型: {protocolType}, id: {msgId}");
            }
            else if (!typeof(IProtocol).IsAssignableFrom(protocolType))
            {
                CommonLog.NetError($"协议生成器注册失败,类型{protocolType}必须继承 IProtocol 接口");
            }
            else
            {
                m_ProtocolTypeDic.Add(msgId, protocolType);
            }
        }

        /// <summary>
        /// 根据消息id生成处理协议
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public IProtocol GenerateProtocol(short msgId)
        {
            if (m_ProtocolTypeDic.TryGetValue(msgId, out var type))
            {
                return (IProtocol)CFM.ReferencePool.Acquire(type);
            }
            else
            {
                CommonLog.NetError($"协议生成器没有注册, id: {msgId}");
                return null;
            }
        }
    }
}
