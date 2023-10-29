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
                Logger.NetError($"和 {Address} 的websocket连接没有完全关闭时尝试重新开始websocket连接");
                return;
            }

            socket = new WebSocket(Address);

            //注册回调
            socket.OnOpen += OnOpen;
            socket.OnClose += OnClose;
            socket.OnError += OnError;
            socket.OnMessage += OnMessage;

            Logger.Net($"websocket开始连接{Address}...");
            socket.ConnectAsync();
        }

        public void SendMsg(string str)
        {
            if (null == socket || socket.ReadyState != WebSocketState.Open)
            {
                Logger.NetError($"和 {Address} 的websocket的连接不可用,无法发送 {str}");
                return;
            }
            socket.SendAsync(Encoding.UTF8.GetBytes(str));
        }

        public void CloseConnect()
        {
            if (null != socket && socket.ReadyState != WebSocketState.Closed && socket.ReadyState != WebSocketState.Closing)
            {
                Logger.Net($"websocket开始关闭{Address}...");
                socket.CloseAsync();
            }
        }

        private void OnOpen(object sender, OpenEventArgs arg)
        {
            Logger.Net($"开启和 {Address} 的websocket连接");
        }

        private void OnClose(object sender, CloseEventArgs arg)
        {
            Logger.Net($"关闭和 {Address} 的websocket连接");
        }

        private void OnError(object sender, ErrorEventArgs arg)
        {
            Logger.NetError($"和 {Address} 的websocket连接出错");
        }

        private void OnMessage(object sender, MessageEventArgs arg)
        {
            if (arg.IsBinary)
            {
                Logger.Net($"websocket 收到来自 {Address} 的信息({arg.RawData.Length}): {arg.Data}");
            }
            else if (arg.IsText)
            {
                Logger.Net($"websocket 收到来自 {Address} 的信息: {arg.Data}");
            }
        }

        private void OnApplicationQuit()
        {
            CloseConnect();
        }
    }
}
