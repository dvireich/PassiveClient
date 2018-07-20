using PassiveShell;
using PostSharp.Patterns.Diagnostics;

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
