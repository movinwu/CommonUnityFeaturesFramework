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

        private WebSocket socket;

        public void StartConnect()
        {
            if (null != socket && socket.ReadyState != WebSocketState.Closed)
            {
                Logger.NetError($"�� {Address} ��websocket����û����ȫ�ر�ʱ�������¿�ʼwebsocket����");
                return;
            }

            socket = new WebSocket(Address);

            //ע��ص�
            socket.OnOpen += OnOpen;
            socket.OnClose += OnClose;
            socket.OnError += OnError;
            socket.OnMessage += OnMessage;

            Logger.Net($"websocket��ʼ����{Address}...");
            socket.ConnectAsync();
        }

        public void SendMsg(string str)
        {
            if (null == socket || socket.ReadyState != WebSocketState.Open)
            {
                Logger.NetError($"�� {Address} ��websocket�����Ӳ�����,�޷����� {str}");
                return;
            }
            socket.SendAsync(Encoding.UTF8.GetBytes(str));
        }

        public void CloseConnect()
        {
            if (null != socket && socket.ReadyState != WebSocketState.Closed && socket.ReadyState != WebSocketState.Closing)
            {
                Logger.Net($"websocket��ʼ�ر�{Address}...");
                socket.CloseAsync();
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
                Logger.Net($"websocket �յ����� {Address} ����Ϣ({arg.RawData.Length}): {arg.Data}");
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
