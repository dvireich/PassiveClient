using PassiveShell;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient
{
    [Log(AttributeExclude = true)]
    public class DownloadFileData
    {
        public string FileName;
        public string Directory;
        public string PathToSaveOnServer;
        public string Id;
        public string TasktId;

        public IPassiveShell ShellService;
    }
}
