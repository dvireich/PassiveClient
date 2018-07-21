using AlertCallBack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Callback_interfaces_and_Implementation
{
    public interface IBaseCallBack : IAletCallBackCallback, IDisposable
    {
        void KeppAlive();
    }
}
