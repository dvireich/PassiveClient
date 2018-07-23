using Authentication;
using PassiveClient.Callback_interfaces_and_Implementation;
using PassiveClient.Helpers;
using PassiveClient.Helpers.Interfaces;
using PassiveShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PassiveClient.Clients
{
    class TestablePassiveClient : PassiveClient
    {
        public TestablePassiveClient(ICallBack callback,
                                     IStatusCallBack status,
                                     IMonitorHelper monitorHelper,
                                     IPassiveShell passiveShell,
                                     IAuthentication authentication,
                                     IShell shell,
                                     ICommunicationExceptionHandler communicationExceptionHandler,
                                     ITransferDataHelper transferDataHelper,
                                     Guid id) : base(passiveShell,
                                                     authentication,
                                                     id)
        {
            this.callback = callback;
            this.status = status;
            this.monitorHelper = monitorHelper;
            this.shell = shell;
            this.communicationExceptionHandler = communicationExceptionHandler;
            this.transferDataHelper = transferDataHelper;
        }

        protected override void CloseAllConnectionsAndDisposeTimers(Timer timerThread)
        {
            try
            {
                if (ShelService != null)
                {
                    ((ICommunicationObject)ShelService).Close();
                }
                if (!Logout(_username, out string error))
                {
                    Console.WriteLine(error);
                }
                if (callback != null)
                {
                    callback.Dispose();
                }
                if (status != null)
                {
                    status.Dispose();
                }
                if (timerThread != null)
                {
                    timerThread.Dispose();
                }
            }
            catch { }
        }


    }
}
