using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OOPS
{
    public class TestNet : MonoBehaviour
    {
        

        private IEnumerator Start()
        {
            NetManager.Instance.StartConnect();
            ProtocolManager.Instance.RegisterProtocol((short)EProtocolId.Heart, typeof(PB_HeartProtocol));
            yield return new WaitForSeconds(5);
            var protocol = ReferencePool.Acquire<PB_HeartProtocol>();
            var data = new PB_Heart();
            data.Id = 0;
            protocol.SendMessage(data);
        }
    }
}
