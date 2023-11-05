using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OOPS
{
    /// <summary>
    /// ͨ�Ź�����,����Э��key������
    /// </summary>
    public class ProtocolManager : SingletonBase<ProtocolManager>
    {
        private ProtocolManager() { }

        /// <summary>
        /// ����Э������
        /// </summary>
        private Dictionary<short, System.Type> m_ProtocolTypeDic = new Dictionary<short, System.Type>();

        /// <summary>
        /// ע��Э�����ɺ���
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="protocolType"></param>
        public void RegisterProtocol(short msgId, System.Type protocolType)
        {
            if (m_ProtocolTypeDic.ContainsKey(msgId))
            {
                Logger.NetError($"Э���������ظ�ע��, ����: {protocolType}, id: {msgId}");
            }
            else if (!typeof(IProtocol).IsAssignableFrom(protocolType))
            {
                Logger.NetError($"Э��������ע��ʧ��,����{protocolType}����̳� IProtocol �ӿ�");
            }
            else
            {
                m_ProtocolTypeDic.Add(msgId, protocolType);
            }
        }

        /// <summary>
        /// ������Ϣid���ɴ���Э��
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public IProtocol GenerateProtocol(short msgId)
        {
            if (m_ProtocolTypeDic.TryGetValue(msgId, out var type))
            {
                return (IProtocol)ReferencePool.Acquire(type);
            }
            else
            {
                Logger.NetError($"Э��������û��ע��, id: {msgId}");
                return null;
            }
        }
    }
}
