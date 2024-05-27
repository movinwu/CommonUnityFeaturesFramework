using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.PSM
{
    public enum EPSMState : byte
    {
        NotInited,

        PrepareToRun,

        Running,

        NotRunning,
    }
}
