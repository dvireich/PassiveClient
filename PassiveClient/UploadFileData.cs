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
    public class UploadFileData
    {
        public RemoteFileInfo Request;
        public string TaskId;
        public string Id;

        public IPassiveShell ShellService;
    }
}
