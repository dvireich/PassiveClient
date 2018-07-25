using PassiveShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers
{
    public interface ITransferDataHelper
    {
        void DownloadFile(DownloadFileData downloadFileData);

        void UploadFile(UploadFileData uploadFileData);

        void SetProxy(IPassiveShell proxy);
    }
}
