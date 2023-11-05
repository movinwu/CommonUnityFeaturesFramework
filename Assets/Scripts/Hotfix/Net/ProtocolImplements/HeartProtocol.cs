using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OOPS
{
    public class HeartProtocol : Protocol<Heart>
    {
        public override short Key => (short)EProtocolId.Heart;

        protected override void OnReceive()
        {
            this.Data.Id = this.Data.Id + 1;
            System.Threading.Tasks.Task.Run(() =>
            {
                System.Threading.Thread.Sleep(2000);
                this.SendMessage();
            });
            //Logger.Trace($"收到心跳消息,id为 {this.Data.Id}");
        }
    }
}
