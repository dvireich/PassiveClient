using AlertCallBack;
using PassiveClient.Callback_Implementation;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using System;
using System.ServiceModel;
using static PassiveClient.PassiveClient;

namespace PassiveClient
{
    public class StatusCallBack : BaseCallBack
    {
        public void SendServerCallBack(string wcfServicesPathId, string id)
        {
            SendServerCallBack(wcfServicesPathId, id, "status");
        }
    }
}
