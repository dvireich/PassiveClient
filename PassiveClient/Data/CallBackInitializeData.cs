using PassiveShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
