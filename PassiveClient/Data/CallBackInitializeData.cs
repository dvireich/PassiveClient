using PassiveShell;
using System;

namespace PassiveClient.Data
{
    public class CallBackInitializeData
    {
        public StatusCallBack StatusCallBack;
        public IPassiveShell Proxy;
        public Action<string> ContinuationError;
        public string NickName;
        public Object ProgramLock;
    }
}
