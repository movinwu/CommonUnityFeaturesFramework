using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityWebSocket;

namespace OOPS
{
    public class NetManager : MonoSingletonBase<NetManager>
    {
        private const string Address = "wss://echo.websocket.events";

        private WebSocket m_Socket;

        public void StartConnect()
        {
            if (null != m_Socket && m_Socket.ReadyState != WebSocketState.Closed)
            {
                Logger.NetError($"�� {Address} ��websocket����û����ȫ�ر�ʱ�������¿�ʼwebsocket����");
                return;
            }

            m_Socket = new WebSocket(Address);

            //ע��ص�
            m_Socket.OnOpen += OnOpen;
            m_Socket.OnClose += OnClose;
            m_Socket.OnError += OnError;
            m_Socket.OnMessage += OnMessage;

            Logger.Net($"websocket��ʼ����{Address}...");
            m_Socket.ConnectAsync();
        }

        public void SendMsg(string str)
        {
            if (null == m_Socket || m_Socket.ReadyState != WebSocketState.Open)
            {
                Logger.NetError($"�� {Address} ��websocket�����Ӳ�����,�޷����� {str}");
                return;
            }
            m_Socket.SendAsync(Encoding.UTF8.GetBytes(str));
        }

        public void SendMsg(byte[] data)
        {
            if (null == m_Socket || m_Socket.ReadyState != WebSocketState.Open)
            {
                Logger.NetError($"�� {Address} ��websocket�����Ӳ�����,�޷����� {Encoding.UTF8.GetString(data)}");
                return;
            }
            m_Socket.SendAsync(data);
        }

        public void CloseConnect()
        {
            if (null != m_Socket && m_Socket.ReadyState != WebSocketState.Closed && m_Socket.ReadyState != WebSocketState.Closing)
            {
                Logger.Net($"websocket��ʼ�ر�{Address}...");
                m_Socket.CloseAsync();
            }
        }

        private void OnOpen(object sender, OpenEventArgs arg)
        {
            Logger.Net($"������ {Address} ��websocket����");
        }

        private void OnClose(object sender, CloseEventArgs arg)
        {
            Logger.Net($"�رպ� {Address} ��websocket����");
        }

        private void OnError(object sender, ErrorEventArgs arg)
        {
            Logger.NetError($"�� {Address} ��websocket���ӳ���");
        }

        private void OnMessage(object sender, MessageEventArgs arg)
        {
            if (arg.IsBinary)
            {
                short msgId = (short)((arg.RawData[0] << 8) + arg.RawData[1]);

                Logger.Net($"websocket �յ����� {Address} ����Ϣ({arg.RawData.Length}), idΪ: {msgId}");

                var protocol = ProtocolManager.Instance.GenerateProtocol(msgId);
                protocol.ReceiveMessage(arg.RawData);
            }
            else if (arg.IsText)
            {
                Logger.Net($"websocket �յ����� {Address} ����Ϣ: {arg.Data}");
            }
        }

        private void OnApplicationQuit()
        {
            CloseConnect();
        }
    }
}
