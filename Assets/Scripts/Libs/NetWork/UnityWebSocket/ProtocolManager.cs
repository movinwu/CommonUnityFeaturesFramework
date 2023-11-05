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
        /// ����Э�����ɺ���
        /// </summary>
        private Dictionary<short, System.Func<IProtocol>> m_ProtocolGenerateFuncDic = new Dictionary<short, System.Func<IProtocol>>();

        /// <summary>
        /// ����Э������
        /// </summary>
        private Dictionary<System.Type, short> m_ProtocolTypeDic = new Dictionary<System.Type, short>();

        /// <summary>
        /// ע��Э�����ɺ���
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="generateFunc"></param>
        /// <param name="protocolType"></param>
        public void RegisterProtocolGenerate(short msgId, System.Func<IProtocol> generateFunc, System.Type protocolType)
        {
            if (m_ProtocolTypeDic.ContainsKey(protocolType))
            {
                Logger.NetError($"Э���������ظ�ע��, ����: {protocolType}, id: {msgId}");
            }
            else if (m_ProtocolGenerateFuncDic.ContainsKey(msgId))
            {
                Logger.NetError($"Э���������ظ�ע��, ����: {protocolType}, id: {msgId}");
            }
            else if (!typeof(Google.Protobuf.IMessage).IsAssignableFrom(protocolType))
            {
                Logger.NetError($"Э��������ע��ʧ��,����{protocolType}������protobuf��������");
            }
            else
            {
                m_ProtocolTypeDic.Add(protocolType, msgId);
                m_ProtocolGenerateFuncDic.Add(msgId, generateFunc);
            }
        }

        /// <summary>
        /// ����id����Э��
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public IProtocol GenerateProtocol(short msgId)
        {
            if (m_ProtocolGenerateFuncDic.TryGetValue(msgId, out var func))
            {
                return func?.Invoke();
            }
            Logger.NetError($"Э��������û��ע��, id: {msgId}");
            return null;
        }

        /// <summary>
        /// ��ȡid
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
                Logger.NetError($"Э��������û��ע��, ����: {protocolType}");
                return -1;
            }
        }
    }
}
