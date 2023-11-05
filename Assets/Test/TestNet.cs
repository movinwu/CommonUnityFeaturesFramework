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
            yield return new WaitForSeconds(5);
            ProtocolManager.Instance.RegisterProtocolGenerate((short)EProtocolId.Heart, () => new HeartProtocol(), typeof(Heart));
            var protocol = ProtocolManager.Instance.GenerateProtocol((short)EProtocolId.Heart);
            if (protocol is HeartProtocol heart)
            {
                var data = new Heart();
                data.Id = 0;
                heart.SetData(data);
                heart.SendMessage();
            }
        }
    }
}
