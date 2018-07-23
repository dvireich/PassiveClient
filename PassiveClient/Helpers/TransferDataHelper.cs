using PassiveClient.Helpers;
using PassiveClient.Helpers.Interfaces;
using PassiveClient.Helpers.Shell.Helpers;
using PassiveClient.Helpers.Shell.Interfaces;
using PassiveShell;
using System;
using System.IO;
using System.Linq;

namespace PassiveClient
{
    public class TransferDataHelper : ITransferDataHelper
    {
        private readonly ICommunicationExceptionHandler _communicationExceptionHandler;
        private readonly IPassiveShell _sellService;
        private readonly IFileInfoHelper _fileHelper;
        private readonly IFileManager _fileManager;
        private readonly IDirectoryManager _directoryManager;

        private const int chunkSize = 999999;

        public TransferDataHelper(ICommunicationExceptionHandler communicationExceptionHandler, 
                                  IPassiveShell sellService,
                                  IFileInfoHelper fileInfoHelper,
                                  IFileManager fileManager,
                                  IDirectoryManager directoryManager)
        {
            _communicationExceptionHandler = communicationExceptionHandler;
            _sellService = sellService;
            _fileHelper = fileInfoHelper;
            _fileManager = fileManager;
            _directoryManager = directoryManager;
        }

        public TransferDataHelper(IPassiveShell sellService) : this(new CommunicationExceptionHandler(), 
                                                                    sellService, 
                                                                    new FileInfoHelper(), 
                                                                    new FileManager(), 
                                                                    new DirectoryManager())
        {
        }

        public void DownloadFile(DownloadFileData downloadFileData)
        {
            var path = Path.Combine(downloadFileData.Directory, downloadFileData.FileName);
            var uploadRequestInfo = new RemoteFileInfo
            {
                FileName = downloadFileData.FileName,
                Length = _fileHelper.GetFileLength(path),
                PathToSaveOnServer = downloadFileData.PathToSaveOnServer,
                taskId = downloadFileData.TasktId,
                id = downloadFileData.Id
            };

            using (var file = _fileManager.OpenRead(path))
            {
                uploadRequestInfo.FileSize = file.Length.ToString();
                int bytesRead;
                var byteStream = new byte[chunkSize * 10];
                while ((bytesRead = file.Read(byteStream, 0, byteStream.Length)) > 0)
                {

                    for (var i = 0; i < bytesRead; i = i + chunkSize)
                    {
                        uploadRequestInfo.FileByteStream = byteStream.Skip(i).Take(chunkSize).ToArray();
                        uploadRequestInfo.FileEnded = false;
                        _communicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                        {
                            _sellService.StartTransferData();
                            _sellService.PassiveDownloadedFile(uploadRequestInfo);
                        });
                    }
                }
                uploadRequestInfo.FileByteStream = new byte[0];
                uploadRequestInfo.FileEnded = true;
                _communicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                {
                    _sellService.StartTransferData();
                    _sellService.PassiveDownloadedFile(uploadRequestInfo);
                });
            }
        }

        public void UploadFile(UploadFileData uploadFileData)
        {
            var requestData = new DownloadRequest
            {
                taskId = uploadFileData.TaskId,
                id = uploadFileData.Request.id
            };
            var endDownload = false;
            using (var fileStrem = CreateNewFile(uploadFileData.Request.FileName, uploadFileData.Request.PathToSaveOnServer))
            {
                if (fileStrem == null)
                {
                    _communicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                    {
                        _sellService.ErrorUploadDownload(uploadFileData.Id, 
                                                         uploadFileData.Request.taskId, 
                                                         string.Format("Fail to create File in your computer {0}", uploadFileData.Request.FileName));
                    });
                    return;
                }

                while (!endDownload)
                {
                    if (uploadFileData.Request.FileName.StartsWith("Error"))
                    {
                        throw new Exception(uploadFileData.Request.FileName);
                    }
                    if (uploadFileData.Request.FileEnded)
                    {
                        endDownload = true;
                        break;
                    }
                    else
                    {
                        var lastByteToWrite = 0;
                        var lastChunk = fileStrem.Position + uploadFileData.Request.FileByteStream.Length >= long.Parse(uploadFileData.Request.FileSize);
                        if (lastChunk)
                            lastByteToWrite = Convert.ToInt32(long.Parse(uploadFileData.Request.FileSize) - fileStrem.Position);
                        fileStrem.Write(uploadFileData.Request.FileByteStream, 0, lastChunk ? lastByteToWrite : uploadFileData.Request.FileByteStream.Length);
                        fileStrem.Flush();
                    }

                    _communicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                    {
                        uploadFileData.Request = _sellService.PassiveGetUploadFile(requestData);
                    });
                }
            }
        }

        private Stream CreateNewFile(string fileName, string pathTosave)
        {
            var dirPath = pathTosave;
            var path = Path.Combine(dirPath, fileName);

            try
            {

                // Delete the file if it exists.
                if (_fileManager.Exists(path))
                {
                    // Note that no lock is put on the
                    // file and the possibility exists
                    // that another process could do
                    // something with it between
                    // the calls to Exists and Delete.
                    _fileManager.Delete(path);
                }

                // Create the file.
                if (!_directoryManager.Exists(dirPath))
                    _directoryManager.CreateDirectory(dirPath);
                var fs = _fileManager.Create(path);
                return fs;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
