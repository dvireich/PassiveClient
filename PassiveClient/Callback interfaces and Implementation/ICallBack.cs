using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Callback_interfaces_and_Implementation
{
    interface ICallBack : IBaseCallBack
    {
       void SendServerCallBack(string wcfServicesPathId, string id);
    }
}
