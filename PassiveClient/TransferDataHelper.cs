using PassiveShell;
using System;
using System.IO;
using System.Linq;

namespace PassiveClient
{
    public static class TransferDataHelper
    {
        public static void DownloadFile(DownloadFileData downloadFileData)
        {
            var path = Path.Combine(downloadFileData.Directory, downloadFileData.FileName);
            FileInfo fileInfo = new FileInfo(path);
            RemoteFileInfo uploadRequestInfo = new RemoteFileInfo();
            uploadRequestInfo.FileName = downloadFileData.FileName;
            uploadRequestInfo.Length = fileInfo.Length;
            uploadRequestInfo.PathToSaveOnServer = downloadFileData.PathToSaveOnServer;
            uploadRequestInfo.taskId = downloadFileData.TasktId;
            uploadRequestInfo.id = downloadFileData.Id;

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
                        CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                        {
                            downloadFileData.ShellService.StartTransferData();
                            downloadFileData.ShellService.PassiveDownloadedFile(uploadRequestInfo);
                        });
                    }
                }
                uploadRequestInfo.FileByteStream = new byte[0];
                uploadRequestInfo.FileEnded = true;
                CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                {
                    downloadFileData.ShellService.StartTransferData();
                    downloadFileData.ShellService.PassiveDownloadedFile(uploadRequestInfo);
                });
            }
        }

        public static void UploadFile(UploadFileData uploadFileData)
        {
            DownloadRequest requestData = new DownloadRequest();
            requestData.taskId = uploadFileData.TaskId;
            requestData.id = uploadFileData.Request.id;
            var endDownload = false;
            using (var fileStrem = CreateNewFile(uploadFileData.Request.FileName, uploadFileData.Request.PathToSaveOnServer))
            {
                if (fileStrem == null)
                {
                    CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                    {
                        uploadFileData.ShellService.ErrorUploadDownload(uploadFileData.Id, uploadFileData.Request.taskId, string.Format("Fail to create File in your computer {0}", uploadFileData.Request.FileName));
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

                    CommunicationExceptionHandler.SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(() =>
                    {
                        uploadFileData.Request = uploadFileData.ShellService.PassiveGetUploadFile(requestData);
                    });
                }
            }
        }

        public static FileStream CreateNewFile(string fileName, string pathTosave)
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
            catch (Exception)
            {
                return null;
            }
        }
    }
}
