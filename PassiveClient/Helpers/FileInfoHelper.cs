using PassiveClient.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers
{
    public class FileInfoHelper : IFileInfoHelper
    {
        public long GetFileLength(string path)
        {
            return new FileInfo(path).Length;
        }
    }
}
