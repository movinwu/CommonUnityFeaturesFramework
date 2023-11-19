using OOPS;

namespace HotfixScripts
{
    public class PB_PersonProtocol : Protocol<PB_Person>
    {
        public override short MsgId => throw new System.NotImplementedException();

        protected override void OnReceive()
        {
            throw new System.NotImplementedException();
        }
    }
}