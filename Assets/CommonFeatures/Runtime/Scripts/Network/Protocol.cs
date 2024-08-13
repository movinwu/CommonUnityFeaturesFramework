using CommonFeatures.Log;
using CommonFeatures.Pool;
using Google.Protobuf;
using System.IO;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// 通信协议
    /// </summary>
    public abstract class Protocol<T> : IProtocol, IReference where T : IMessage<T>
    {
        public abstract short MsgId { get; }

        /// <summary>
        /// proto数据
        /// </summary>
        protected T Data { get; private set; }

        public void ReceiveMessage(byte[] data)
        {
            if (null == this.Data)
            {
                this.Data = System.Activator.CreateInstance<T>();
            }

            try
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    ms.Write(data, 2, data.Length - 2);
                    ms.Position = 0;
                    this.Data.MergeFrom(ms);
                }

                this.OnReceive();
            }
            catch (System.Exception ex)
            {
                CommonLog.NetError($"消息反序列化失败, 类型为: {this.Data.GetType()}");
                CommonLog.NetError(ex);
            }
        }

        public void SendMessage(byte[] data)
        {
            CFM.Network.SendMsg(data);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendMessage(T data)
        {
            try
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    this.Data = data;
                    this.Data.WriteTo(ms);
                    byte[] buffer = new byte[ms.Length + 2];
                    var msgId = this.MsgId;
                    buffer[0] = (byte)(msgId >> 8);
                    buffer[1] = (byte)msgId;
                    ms.Position = 0;
                    ms.Read(buffer, 2, (int)ms.Length);
                    SendMessage(buffer);
                }
            }
            catch (System.Exception ex)
            {
                CommonLog.NetError($"消息序列化失败,类型为: {this.Data.GetType()}");
                CommonLog.NetError(ex);
            }
            finally
            {
                CFM.ReferencePool.Back(this);
            }
        }

        /// <summary>
        /// 当收到消息时(处理消息)
        /// </summary>
        protected abstract void OnReceive();

        public void Reset()
        {
            
        }
    }
}
