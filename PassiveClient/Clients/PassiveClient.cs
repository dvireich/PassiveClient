using Authentication;
using PassiveClient.Callback_interfaces_and_Implementation;
using PassiveClient.Helpers;
using PassiveClient.Helpers.Interfaces;
using PassiveShell;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]


namespace PassiveClient
{
    [Log(AttributeTargetElements= MulticastTargets.Method, AttributeTargetTypeAttributes= MulticastAttributes.Public, AttributeTargetMemberAttributes= MulticastAttributes.Private | MulticastAttributes.Public)]
    public class PassiveClient : AuthenticationClient
    {

        private ICallBack callback;
        private IStatusCallBack status;
        private IMonitorHelper monitorHelper;
        private readonly IShell shell;
        private readonly ICommunicationExceptionHandler communicationExceptionHandler;
        private ITransferDataHelper transferDataHelper;

        private bool _firstTimeSucceededToSubscribe = true;
        private readonly Object _endProgram = new Object();
        private bool _shouldRestartConnections = false;

        public PassiveClient(ICallBack callback,
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

        public PassiveClient()
        {
            monitorHelper = new MonitorHelper();
            shell = new CSharpShell();
            communicationExceptionHandler = new CommunicationExceptionHandler();
        }
        
        public void Main(string[] args)
        {
            string nickName = string.Empty;

            PrintArgsUsage();
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i].Split('=').First();
                    var val = args[i].Split('=').Last();
                    if (arg.ToLower() == "nickname")
                    {
                        nickName = val;
                    }
                    if (arg.ToLower() == "username")
                    {
                        _username = val;
                    }
                    if (arg.ToLower() == "password")
                    {
                        _password = val;
                    }
                }
            }

            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
                throw new Exception("Cant connect without user name and password. please run with cmd args userName=? password=?");

            InitializeServiceAndWaitForCommands(nickName);
            Console.WriteLine("Exited...");
        }

        private static void PrintArgsUsage()
        {
            Console.WriteLine("The command line arguments for this program is:");
            Console.WriteLine("nickname=<some name>");
            Console.WriteLine("username=<user name>");
            Console.WriteLine("password=<password>");
        }

        public void InitializeServiceAndWaitForCommands(string nickName)
        {
            Timer timerThread = null;
            do
            {
                try
                {
                    if (!Authenticate(_username, _password, out string error, out _wcfServicesPathId))
                    {
                        throw new Exception(string.Format("Could not Authenticate username {0} and pssword {1} with the following error {2}", _username, _password, error));
                    }

                    ShelService = ShelService ?? (IPassiveShell)InitializeServiceReferences<IPassiveShell>();
                    transferDataHelper = transferDataHelper ?? new TransferDataHelper(ShelService);

                    CleanPreviousPassiveClientId();

                    WaitUntilSubscribed(nickName);

                    InitializeCallBackCommunicationClient(nickName, _endProgram, (nickname) =>
                    {
                        CloseAllConnectionsAndDisposeTimers(timerThread);
                        InitializeServiceAndWaitForCommands(nickname);
                    });

                    timerThread = StartKeepAliveCallbackThread(callback, status);

                    Console.WriteLine(string.Format("Subscribed with id: {0}", Id));
                    Console.WriteLine("Wating for command");

                    //Make this thrad wait untill there is a CloseClient command from the active client
                    WaitUntilActiveClientCloseOrLostConnections();

                    CloseAllConnectionsAndDisposeTimers(timerThread);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Server is not responding with this error: {e.Message}");

                    CloseAllConnectionsAndDisposeTimers(timerThread);

                    Thread.Sleep(1000);

                    _shouldRestartConnections = true;
                }
            } while (_shouldRestartConnections);
        }

        private void CloseAllConnectionsAndDisposeTimers(Timer timerThread)
        {
            try
            {
                if (ShelService != null)
                {
                    ((ICommunicationObject)ShelService).Close();
                    ShelService = null;
                }
                if (!Logout(_username, out string error))
                {
                    Console.WriteLine(error);
                }
                if(callback != null)
                {
                    callback.Dispose();
                    callback = null;
                }
                if(status != null)
                {
                    status.Dispose();
                    status = null;
                }
               if(timerThread != null)
                {
                    timerThread.Dispose();
                    timerThread = null;
                }
            }
            catch { }
        }

        private void WaitUntilActiveClientCloseOrLostConnections()
        {
            lock (_endProgram)
            {
                _shouldRestartConnections = false;
                monitorHelper.Wait(_endProgram);
            }
        }

        private void WaitUntilSubscribed(string nickName)
        {
            while (!ShelService.Subscribe(Id.ToString(), Constants.virsion, nickName))
            {
                Console.WriteLine(string.Format("Wasn't able to connect with {0} id, tring diffrent one...", Id));
                CleanPreviousPassiveClientId();
                //maybe this guid is taken, try new one
                Id = Guid.NewGuid();
            }
            _firstTimeSucceededToSubscribe = false;
        }

        private Timer StartKeepAliveCallbackThread(ICallBack callback, IStatusCallBack statusCallback)
        {
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(45);

            var timer = new Timer((e) =>
            {
                if (_shouldRestartConnections) return;

                var numberOfTries = 3;
                //maybe the server is down. If so we will close the connection and will reset all the connections;
                while (numberOfTries > 0)
                {
                    try
                    {
                        callback.KeppAlive();
                        statusCallback.KeppAlive();
                        break;
                    }
                    catch
                    {
                        numberOfTries--;
                    }
                }

                if (numberOfTries == 0)
                {
                    _shouldRestartConnections = true;
                    lock (_endProgram)
                    {
                        monitorHelper.PulseAll(_endProgram);
                    }
                }

            }, null, startTimeSpan, periodTimeSpan);
            return timer;
        }

        private void CleanPreviousPassiveClientId()
        {
            if (_firstTimeSucceededToSubscribe) return;

            var succeeded = false;
            while (!succeeded)
            {
                try
                {
                    ShelService.RemoveId(Id.ToString());
                    succeeded = true;
                }
                catch
                {
                    Console.WriteLine("Server not responding. Waiting tor server to respond...");
                    Thread.Sleep(1000);
                }
            }
        }

        private void InitializeCallBackCommunicationClient(string nickName, Object programLock, Action<string> onContinuationError)
        {

            if (callback != null && status != null) return;

            var succeed = false;
            while (!succeed)
            {
                try
                {
                    status = new StatusCallBack();
                    callback = new CallBack(ShelService,
                                            status,
                                            shell,
                                            communicationExceptionHandler,
                                            new TransferDataHelper(ShelService),
                                            monitorHelper,
                                            nickName,
                                            programLock,
                                            onContinuationError);

                    status.SendServerCallBack(_wcfServicesPathId, Id.ToString());
                    callback.SendServerCallBack(_wcfServicesPathId, Id.ToString());
                    succeed = true;
                }
                catch
                {
                    if (callback != null) callback.Dispose();
                    if (status != null)   status.Dispose();
                    Console.WriteLine("Error Register CallBacks, Disposing and trying again");
                }
            }
        }
    }
}
