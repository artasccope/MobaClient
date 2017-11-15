
namespace GameFW.Core.Msg
{
    public class MsgAccount : MsgBase
    {
        public MsgAccount(ushort msgId, string account, string password)
        {
            this.msgId = msgId;
            this.account = account;
            this.password = password;
        }

        public MsgAccount() { }

        protected string account;
        protected string password;

        public string Account
        {
            get { return account; }
        }

        public string Password
        {
            get { return password; }
        }

        public void SetMsgAccount(ushort msgId, string account, string password)
        {
            this.msgId = msgId;
            this.account = account;
            this.password = password;
        }
    }
}
