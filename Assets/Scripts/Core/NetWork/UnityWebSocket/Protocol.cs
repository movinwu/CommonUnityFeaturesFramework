using Google.Protobuf;
using System.IO;

namespace OOPS
{
    /// <summary>
    /// ͨ��Э��
    /// </summary>
    public abstract class Protocol<T> : IProtocol where T : IMessage<T>
    {
        public abstract short Key { get; }

        /// <summary>
        /// proto����
        /// </summary>
        protected T Data { get; private set; }

        public void SetData(T data)
        {
            this.Data = data;
        }

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
                Logger.NetError($"��Ϣ�����л�ʧ��, ����Ϊ: {this.Data.GetType()}");
                Logger.NetException(ex);
            }
        }

        public void SendMessage(byte[] data)
        {
            NetManager.Instance.SendMsg(data);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="msg"></param>
        public void SendMessage()
        {
            try
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    this.Data.WriteTo(ms);
                    byte[] buffer = new byte[ms.Length + 2];
                    var msgId = ProtocolManager.Instance.GetProtocolId(this.Data.GetType());
                    buffer[0] = (byte)(msgId >> 8);
                    buffer[1] = (byte)msgId;
                    ms.Position = 0;
                    ms.Read(buffer, 2, (int)ms.Length);
                    SendMessage(buffer);
                }
            }
            catch (System.Exception ex)
            {
                Logger.NetError($"��Ϣ���л�ʧ��,����Ϊ: {this.Data.GetType()}");
                Logger.NetException(ex);
            }
        }

        /// <summary>
        /// ���յ���Ϣʱ(������Ϣ)
        /// </summary>
        protected abstract void OnReceive();
    }
}
