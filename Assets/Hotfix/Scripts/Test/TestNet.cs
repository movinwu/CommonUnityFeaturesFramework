using CommonFeatures.NetWork;
using HotfixScripts;
using System.Collections;
using UnityEngine;

namespace CommonFeatures.Test
{
    public class TestNet : MonoBehaviour
    {


        private IEnumerator Start()
        {
            CFM.Network.StartConnect();
            ProtocolManager.Instance.RegisterProtocol((short)EProtocolId.Heart, typeof(PB_HeartProtocol));
            yield return new WaitForSeconds(5);
            var protocol = CFM.ReferencePool.Acquire<PB_HeartProtocol>();
            var data = new PB_Heart();
            data.Id = 0;
            protocol.SendMessage(data);
        }
    }
}
