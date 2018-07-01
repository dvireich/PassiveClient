using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClientService
{
    public partial class PassiveClientService : ServiceBase
    {
        Process p;
        public PassiveClientService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var argsArr = GetArgsFromFile();
            p = Process.Start(new ProcessStartInfo()
            {
                UseShellExecute = false,
                FileName = "PassiveClient.exe",
                Arguments = string.Join(" ", argsArr),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            });
        }

        private List<string> GetArgsFromFile()
        {
            var argsList = new List<string> {"hidden=true",
                                            "morethanoneclinet=true"};

            var AssemblyDir = Directory.GetParent(Assembly.GetExecutingAssembly().Location);
            var dataPath = $"{AssemblyDir}\\Services\\{GetServiceName()}\\StartData";
            var fileData = File.ReadAllText(dataPath);
            var args = fileData.Split(' ');
            argsList.AddRange(args);
            return argsList;
        }

        private string GetServiceName()
        {
            return GetLastModifiedFolderInServicesDir();
        }

        private string GetLastModifiedFolderInServicesDir()
        {
            var AssemblyDir = Directory.GetParent(Assembly.GetExecutingAssembly().Location);
            var directory = new DirectoryInfo(Path.Combine(AssemblyDir.FullName, "Services"));
            var lastModifiedFolder = (from f in directory.GetDirectories()
                                      orderby f.LastWriteTime descending
                                      select f).First();

            return lastModifiedFolder.Name;
        }

        protected override void OnStop()
        {
            p.Kill();
        }
    }
}
