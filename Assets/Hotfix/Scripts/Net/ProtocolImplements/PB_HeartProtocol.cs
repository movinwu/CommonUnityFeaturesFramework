//------------------------------------------------------------
// oops-framework-cocos2unity
// Copyright © 2023 movinwu. All rights reserved.
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2023/11/19 22:03:14
//------------------------------------------------------------

using CommonFeatures.Log;
using CommonFeatures.NetWork;
using CommonFeatures.Pool;

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
            CommonLog.Log($"收到心跳消息,id为 {this.Data.Id}");
        }
    }
}