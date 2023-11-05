using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OOPS
{
    public class HeartProtocol : Protocol<PB_Heart>
    {
        public override short MsgId => (short)EProtocolId.Heart;

        protected override void OnReceive()
        {
            var id = this.Data.Id + 1;
            System.Threading.Tasks.Task.Run(() =>
            {
                System.Threading.Thread.Sleep(2000);
                var heart = ReferencePool.Acquire<HeartProtocol>();//ÿ�ζ�ȡ����д����Ϣ������ӻ������ȡ���µ�item
                var data = new PB_Heart();
                data.Id = id;
                heart.SendMessage(data);
            });
            Logger.Trace($"�յ�������Ϣ,idΪ {this.Data.Id}");
        }
    }
}
