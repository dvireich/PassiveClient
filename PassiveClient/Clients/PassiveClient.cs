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
        
        private bool _firstTimeSucceededToSubscribe = true;
        private readonly Object _endProgram = new Object();
        private bool _shouldRestartConnections = false;

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

                    _shelService = (IPassiveShell)initializeServiceReferences<IPassiveShell>();

                    CleanPreviousPassiveClientId();

                    WaitUntilSubscribed(nickName);

                    InitializeCallBackCommunicationClient(nickName, _endProgram, (nickname) =>
                    {
                        CloseAllConnectionsAndDisposeTimers(timerThread);
                        InitializeServiceAndWaitForCommands(nickname);
                    });

                    timerThread = StartKeepAliveCallbackThread(callback, status);

                    Console.WriteLine(string.Format("Subscribed with id: {0}", id));
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
                if (_shelService != null)
                {
                    ((ICommunicationObject)_shelService).Close();
                    _shelService = null;
                }
                if (!Logout(_username, out string error))
                {
                    Console.WriteLine(error);
                }
                if(callback != null)
                {
                    callback.Dispose();
                }
                if(status != null)
                {
                    status.Dispose();
                }
               if(timerThread != null)
                {
                    timerThread.Dispose();
                }
            }
            catch { }
        }

        private void WaitUntilActiveClientCloseOrLostConnections()
        {
            lock (_endProgram)
            {
                _shouldRestartConnections = false;
                Monitor.Wait(_endProgram);
            }
        }

        private void WaitUntilSubscribed(string nickName)
        {
            while (!_shelService.Subscribe(id.ToString(), Constants.virsion, nickName))
            {
                Console.WriteLine(string.Format("Wasn't able to connect with {0} id, tring diffrent one...", id));
                CleanPreviousPassiveClientId();
                //maybe this guid is taken, try new one
                id = Guid.NewGuid();
            }
            _firstTimeSucceededToSubscribe = false;
        }

        private Timer StartKeepAliveCallbackThread(CallBack callback, StatusCallBack statusCallback)
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
                        Monitor.PulseAll(_endProgram);
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
                    _shelService.RemoveId(id.ToString());
                    succeeded = true;
                }
                catch
                {
                    Console.WriteLine("Server not responding. Waiting tor server to respond...");
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
