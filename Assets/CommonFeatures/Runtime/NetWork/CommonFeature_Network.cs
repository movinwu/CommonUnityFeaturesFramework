using CommonFeatures.Pool;
using System.Text;
using UnityWebSocket;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// 通用功能-网络长连接
    /// </summary>
    public class CommonFeature_Network : CommonFeature
    {
        private const string Address = "wss://echo.websocket.events";

        private WebSocket m_Socket;

        public void StartConnect()
        {
            if (null != m_Socket && m_Socket.ReadyState != WebSocketState.Closed)
            {
                CommonFeatures.Log.CommonLog.NetError($"和 {Address} 的websocket连接没有完全关闭时尝试重新开始websocket连接");
                return;
            }

            m_Socket = new WebSocket(Address);

            //注册回调
            m_Socket.OnOpen += OnOpen;
            m_Socket.OnClose += OnClose;
            m_Socket.OnError += OnError;
            m_Socket.OnMessage += OnMessage;

            CommonFeatures.Log.CommonLog.Net($"websocket开始连接{Address}...");
            m_Socket.ConnectAsync();
        }

        public void SendMsg(string str)
        {
            if (null == m_Socket || m_Socket.ReadyState != WebSocketState.Open)
            {
                CommonFeatures.Log.CommonLog.NetError($"和 {Address} 的websocket的连接不可用,无法发送 {str}");
                return;
            }
            m_Socket.SendAsync(Encoding.UTF8.GetBytes(str));
        }

        public void SendMsg(byte[] data)
        {
            if (null == m_Socket || m_Socket.ReadyState != WebSocketState.Open)
            {
                CommonFeatures.Log.CommonLog.NetError($"和 {Address} 的websocket的连接不可用,无法发送 {Encoding.UTF8.GetString(data)}");
                return;
            }
            m_Socket.SendAsync(data);
        }

        public void CloseConnect()
        {
            if (null != m_Socket && m_Socket.ReadyState != WebSocketState.Closed && m_Socket.ReadyState != WebSocketState.Closing)
            {
                CommonFeatures.Log.CommonLog.Net($"websocket开始关闭{Address}...");
                m_Socket.CloseAsync();
            }
        }

        private void OnOpen(object sender, OpenEventArgs arg)
        {
            CommonFeatures.Log.CommonLog.Net($"开启和 {Address} 的websocket连接");
        }

        private void OnClose(object sender, CloseEventArgs arg)
        {
            CommonFeatures.Log.CommonLog.Net($"关闭和 {Address} 的websocket连接");
        }

        private void OnError(object sender, ErrorEventArgs arg)
        {
            CommonFeatures.Log.CommonLog.NetError($"和 {Address} 的websocket连接出错");
        }

        private void OnMessage(object sender, MessageEventArgs arg)
        {
            if (arg.IsBinary)
            {
                short msgId = (short)((arg.RawData[0] << 8) + arg.RawData[1]);

                CommonFeatures.Log.CommonLog.Net($"websocket 收到来自 {Address} 的信息({arg.RawData.Length}), id为: {msgId}");

                var protocol = ProtocolManager.Instance.GenerateProtocol(msgId);
                protocol.ReceiveMessage(arg.RawData);
                ReferencePool.Back((IReference)protocol);
            }
            else if (arg.IsText)
            {
                CommonFeatures.Log.CommonLog.Net($"websocket 收到来自 {Address} 的信息: {arg.Data}");
            }
        }

        private void OnApplicationQuit()
        {
            CloseConnect();
        }
    }
}
