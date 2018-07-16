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
        public static Guid id = Guid.NewGuid();
        static bool _firstTimeSucceededToSubscribe = true;
        static volatile Object endProgram = new Object();
        private string _currentTasktId;
        public static string _passiveClientNickName = string.Empty;
        private static bool shouldRestartConnections = false;
        
        public void Main(string[] args)
        {
            string username = string.Empty;
            string password = string.Empty;

            PrintArgsUsage();
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i].Split('=').First();
                    var val = args[i].Split('=').Last();
                    if (arg.ToLower() == "nickname")
                    {
                        _passiveClientNickName = val;
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

            InitializeServiceAndWaitForCommands();
            Console.WriteLine("Exited...");
        }

        private static void PrintArgsUsage()
        {
            Console.WriteLine("The command line arguments for this program is:");
            Console.WriteLine("nickname=<some name>");
            Console.WriteLine("username=<user name>");
            Console.WriteLine("password=<password>");
        }

        public void InitializeServiceAndWaitForCommands()
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

                    WaitUntilSubscribed();

                    RegisterCallBackCommunicationClient(CheckMissions, () =>
                    {
                        CloseAllConnectionsAndDisposeTimers(timerThread);
                        InitializeServiceAndWaitForCommands();
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
                    shouldRestartConnections = true;
                }
            } while (shouldRestartConnections);
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

        private static void WaitUntilActiveClientCloseOrLostConnections()
        {
            lock (endProgram)
            {
                shouldRestartConnections = false;
                Monitor.Wait(endProgram);
            }
        }

        private void WaitUntilSubscribed()
        {
            while (!_shelService.Subscribe(id.ToString(), Constants.virsion, _passiveClientNickName))
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
                if (shouldRestartConnections) return;

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
                    shouldRestartConnections = true;
                    lock (endProgram)
                    {
                        Monitor.PulseAll(endProgram);
                    }
                }

            }, null, startTimeSpan, periodTimeSpan);
            return timer;
        }

        public void CheckMissions(Shell shellHandler)
        {
            if (CheckIfHasShellCommand())
            {
                ShellCommandHendler(shellHandler);
            }
            else if (CheckIfHasDownloadCommand())
            {
                DownloadCommandHendler();
            }
            else if (CheckIfHasUploadCommand())
            {
                UploadCommandHendler();
            }  
        }

        private void UploadCommandHendler()
        {
            try
            {
                var currentId = id;
                var uploadCommand = CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.PassiveGetUploadFile(new DownloadRequest
                {
                    id = currentId.ToString()
                }));
                //In case that between the has command to the get command the task has been deleted
                //the sign is the empty Guid
                if (uploadCommand.taskId == Guid.Empty.ToString())
                    return;
                _currentTasktId = uploadCommand.taskId;
                
                TransferDataHelper.UploadFile(new UploadFileData()
                {
                    Id = id.ToString(),
                    TaskId = _currentTasktId,
                    Request = uploadCommand,
                    ShellService = _shelService
                });

                CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.PassiveUploadedFile(id.ToString(), _currentTasktId));
            }

            catch (Exception e)
            {
                CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                _shelService.ErrorUploadDownload(id.ToString(), _currentTasktId, e.Message));
            }
        }

        private bool CheckIfHasUploadCommand()
        {
            return CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.HasUploadCommand(id.ToString()));
        }

        private bool CheckIfHasDownloadCommand()
        {
            return CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.HasDownloadCommand(id.ToString()));
        }

        private bool CheckIfHasShellCommand()
        {
            return CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.HasCommand(id.ToString()));
        }

        private void ShellCommandHendler(Shell shellHandler)
        {
            try
            {
                ProcessNextCommand(shellHandler);
            }
            catch (Exception e)
            {
                CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                    _shelService.ErrorNextCommand(id.ToString(), _currentTasktId, string.Format("Error Next-Command: {0}", e.Message)));
            }
        }

        private void DownloadCommandHendler()
        {
            try
            {
                var currentId = id;
                var downloadCommand = CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.PassiveGetDownloadFile(new DownloadRequest
                {
                    id = currentId.ToString()
                }));
                //In case that between the has command to the get command the task has been deleted
                //the sign is the empty Guid
                if (downloadCommand.taskId == Guid.Empty.ToString())
                    return;
                _currentTasktId = downloadCommand.taskId;
                var downloadFileName = downloadCommand.FileName;
                var downloadDirectory = downloadCommand.PathInServer;
                var downloadPathToSave = downloadCommand.PathToSaveInClient;

                
                TransferDataHelper.DownloadFile(new DownloadFileData()
                {
                    FileName = downloadFileName,
                    Directory = downloadDirectory,
                    PathToSaveOnServer = downloadPathToSave,
                    Id = id.ToString(),
                    TasktId = _currentTasktId,
                    ShellService = _shelService
                });
            }
            catch (Exception e)
            {
                CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                    _shelService.ErrorUploadDownload(id.ToString(), _currentTasktId, e.Message));
            }
        }

        private void CleanPreviousPassiveClientId()
        {
            if (_firstTimeSucceededToSubscribe) return;

            var succeeded = false;
            while (!succeeded)
            {
                try
                {
                    _shelService.CommandResponse(id.ToString(), _currentTasktId, "CleanId");
                    succeeded = true;
                }
                catch
                {
                    Console.WriteLine("Server not responding. Waiting tor server to respond...");
                    Thread.Sleep(1000);
                }
            }
        }

        private void ProcessNextCommand(Shell shellHandler)
        {
            var command = CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.PassiveNextCommand(id.ToString()));
            _currentTasktId = command.Item3;
            switch (command.Item1)
            {
                case Constants.run:
                    CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                        _shelService.PassiveClientRun(id.ToString(), _currentTasktId, shellHandler.Run()));
                    break;
                case Constants.nextCommand:
                    CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                        _shelService.CommandResponse(id.ToString(), _currentTasktId, shellHandler.NextCommand(command.Item2)));
                    break;
                case Constants.closeShell:
                    shellHandler.CloseShell();
                    CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                        _shelService.CommandResponse(id.ToString(), _currentTasktId, "EndProg"));
                    lock (endProgram)
                    {
                        Monitor.PulseAll(endProgram);
                    }
                    break;
                //In case that between the has command to the get command the task has been deleted
                //the sign is the empty string send by the server
                default:
                    break;
            }
        }
    }
}
