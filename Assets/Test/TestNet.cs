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
            NetManager.Instance.SendMsg("≤‚ ‘∑¢ÀÕ");
        }
    }
}
