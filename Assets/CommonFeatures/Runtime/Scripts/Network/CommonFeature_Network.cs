using CommonFeatures.Pool;
using System.Text;
using UnityWebSocket;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// ͨ�ù���-���糤����
    /// </summary>
    public class CommonFeature_Network : CommonFeature
    {
        private const string Address = "wss://echo.websocket.events";

        private WebSocket m_Socket;

        public void StartConnect()
        {
            if (null != m_Socket && m_Socket.ReadyState != WebSocketState.Closed)
            {
                CommonFeatures.Log.CommonLog.NetError($"�� {Address} ��websocket����û����ȫ�ر�ʱ�������¿�ʼwebsocket����");
                return;
            }

            m_Socket = new WebSocket(Address);

            //ע��ص�
            m_Socket.OnOpen += OnOpen;
            m_Socket.OnClose += OnClose;
            m_Socket.OnError += OnError;
            m_Socket.OnMessage += OnMessage;

            CommonFeatures.Log.CommonLog.Net($"websocket��ʼ����{Address}...");
            m_Socket.ConnectAsync();
        }

        public void SendMsg(string str)
        {
            if (null == m_Socket || m_Socket.ReadyState != WebSocketState.Open)
            {
                CommonFeatures.Log.CommonLog.NetError($"�� {Address} ��websocket�����Ӳ�����,�޷����� {str}");
                return;
            }
            m_Socket.SendAsync(Encoding.UTF8.GetBytes(str));
        }

        public void SendMsg(byte[] data)
        {
            if (null == m_Socket || m_Socket.ReadyState != WebSocketState.Open)
            {
                CommonFeatures.Log.CommonLog.NetError($"�� {Address} ��websocket�����Ӳ�����,�޷����� {Encoding.UTF8.GetString(data)}");
                return;
            }
            m_Socket.SendAsync(data);
        }

        public void CloseConnect()
        {
            if (null != m_Socket && m_Socket.ReadyState != WebSocketState.Closed && m_Socket.ReadyState != WebSocketState.Closing)
            {
                CommonFeatures.Log.CommonLog.Net($"websocket��ʼ�ر�{Address}...");
                m_Socket.CloseAsync();
            }
        }

        private void OnOpen(object sender, OpenEventArgs arg)
        {
            CommonFeatures.Log.CommonLog.Net($"������ {Address} ��websocket����");
        }

        private void OnClose(object sender, CloseEventArgs arg)
        {
            CommonFeatures.Log.CommonLog.Net($"�رպ� {Address} ��websocket����");
        }

        private void OnError(object sender, ErrorEventArgs arg)
        {
            CommonFeatures.Log.CommonLog.NetError($"�� {Address} ��websocket���ӳ���");
        }

        private void OnMessage(object sender, MessageEventArgs arg)
        {
            if (arg.IsBinary)
            {
                short msgId = (short)((arg.RawData[0] << 8) + arg.RawData[1]);

                CommonFeatures.Log.CommonLog.Net($"websocket �յ����� {Address} ����Ϣ({arg.RawData.Length}), idΪ: {msgId}");

                var protocol = ProtocolManager.Instance.GenerateProtocol(msgId);
                protocol.ReceiveMessage(arg.RawData);
                ReferencePool.Back((IReference)protocol);
            }
            else if (arg.IsText)
            {
                CommonFeatures.Log.CommonLog.Net($"websocket �յ����� {Address} ����Ϣ: {arg.Data}");
            }
        }

        private void OnApplicationQuit()
        {
            CloseConnect();
        }
    }
}
