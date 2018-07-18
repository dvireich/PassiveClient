using AlertCallBack;
using PassiveClient.Data;
using PassiveShell;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using static PassiveClient.PassiveClient;

namespace PassiveClient
{
    [Log(AttributeTargetElements = MulticastTargets.Method, AttributeTargetTypeAttributes = MulticastAttributes.Public, AttributeTargetMemberAttributes = MulticastAttributes.Private | MulticastAttributes.Public)]
    public class CallBack : IAletCallBackCallback, IDisposable
    {
        private IPassiveShell _shelService;
        private AletCallBackClient _proxy;
        private Action<string> _onError;
        private Shell _shellHandler;
        private StatusCallBack _statusCallBack;
        private bool _isDead = false;
        private string _id;
        private string _currentTasktId;
        private Object _programLock;
        private string _nickName;


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

        public void Initialize(CallBackInitializeData data)
        {
            _statusCallBack = data.StatusCallBack;
            _shellHandler = new Shell();
            _shelService = data.Proxy;
            _onError = data.ContinuationError;
            _programLock = data.ProgramLock;
            _nickName = data.NickName;
        }

        public void SendServerCallBack(string wcfServicesPathId, string id)
        {
            _id = id;

            Uri endPointAdress = new Uri(string.Format("net.tcp://localhost/ShellTrasferServer/CallBack/{0}", wcfServicesPathId));
            NetTcpBinding wsd = new NetTcpBinding();
            wsd.Security.Mode = SecurityMode.None;
            wsd.CloseTimeout = TimeSpan.MaxValue;
            wsd.ReceiveTimeout = TimeSpan.MaxValue;
            wsd.OpenTimeout = TimeSpan.MaxValue;
            wsd.SendTimeout = TimeSpan.MaxValue;
            EndpointAddress ea = new EndpointAddress(endPointAdress);
            _proxy = new AletCallBackClient(new InstanceContext(this), wsd, ea);
            _proxy.InnerDuplexChannel.OperationTimeout = TimeSpan.MaxValue;
            _proxy.InnerChannel.OperationTimeout = TimeSpan.MaxValue;
            _proxy.RegisterCallBackFunction(id, "Main");
        }

        public void CallBackFunction(string str)
        {
            if (str == "livnessCheck") return;

            if (str.Split(' ').First().ToLower() == "nickname")
            {
                _nickName = str.Split(' ').Last();
            }
               
            try
            {
                CheckMissions(_shellHandler);
            }
            catch
            {
                Dispose();
                _statusCallBack.Dispose();
                _onError(_nickName);
            }
        }

        public void Dispose()
        {
            try
            {
                _isDead = true;
                if (_proxy != null && _proxy.State == CommunicationState.Opened)
                    _proxy.Close();
            }
            catch { }
            
        }

        public void KeppAlive()
        {
            if (!_isDead)
                _proxy.KeepCallBackAlive(_id);
        }

        public void KeepCallbackALive()
        {
            //Do nothing, this function is only for the server in order to send traffic through the connection pipe  
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
                var currentId = _id;
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
                    Id = _id,
                    TaskId = _currentTasktId,
                    Request = uploadCommand,
                    ShellService = _shelService
                });

                CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.PassiveUploadedFile(_id, _currentTasktId));
            }

            catch (Exception e)
            {
                CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                _shelService.ErrorUploadDownload(_id, _currentTasktId, e.Message));
            }
        }

        private bool CheckIfHasUploadCommand()
        {
            return CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.HasUploadCommand(_id));
        }

        private bool CheckIfHasDownloadCommand()
        {
            return CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.HasDownloadCommand(_id));
        }

        private bool CheckIfHasShellCommand()
        {
            return CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.HasCommand(_id));
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
                    _shelService.ErrorNextCommand(_id, _currentTasktId, string.Format("Error Next-Command: {0}", e.Message)));
            }
        }

        private void DownloadCommandHendler()
        {
            try
            {
                var currentId = _id;
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
                    Id = _id,
                    TasktId = _currentTasktId,
                    ShellService = _shelService
                });
            }
            catch (Exception e)
            {
                CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                    _shelService.ErrorUploadDownload(_id, _currentTasktId, e.Message));
            }
        }

        private void ProcessNextCommand(Shell shellHandler)
        {
            var command = CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.PassiveNextCommand(_id));
            _currentTasktId = command.Item3;
            switch (command.Item1)
            {
                case Constants.run:
                    CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                        _shelService.PassiveClientRun(_id, _currentTasktId, shellHandler.Run()));
                    break;
                case Constants.nextCommand:
                    CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                        _shelService.CommandResponse(_id, _currentTasktId, shellHandler.NextCommand(command.Item2)));
                    break;
                case Constants.closeShell:
                    shellHandler.CloseShell();
                    CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                        _shelService.CommandResponse(_id, _currentTasktId, "EndProg"));
                    lock (_programLock)
                    {
                        Monitor.PulseAll(_programLock);
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
