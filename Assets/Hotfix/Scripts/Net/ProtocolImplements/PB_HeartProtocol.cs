using OOPS;

namespace HotfixScripts
{
    public class PB_HeartProtocol : Protocol<PB_Heart>
    {
        public override short MsgId => (short)EProtocolId.Heart;

        protected override void OnReceive()
        {
            var id = this.Data.Id + 1;
            System.Threading.Tasks.Task.Run(() =>
            {
                System.Threading.Thread.Sleep(2000);
                var heart = ReferencePool.Acquire<PB_HeartProtocol>();//每次读取或者写入消息都必须从缓存池中取用新的item
                var data = new PB_Heart();
                data.Id = id;
                heart.SendMessage(data);
            });
            Logger.Trace($"收到心跳消息,id为 {this.Data.Id}");
        }
    }
}