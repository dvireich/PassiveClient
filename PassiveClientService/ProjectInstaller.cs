using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace PassiveClientService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.PassiveClientInstaller.ServiceName = GetServiceName();
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

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            try
            {
                using (ServiceController sc = new ServiceController(PassiveClientInstaller.ServiceName, Environment.MachineName))
                {
                    if (sc.Status != ServiceControllerStatus.Running)
                        sc.Start();
                }
            }
            catch (Exception ee)
            {
                EventLog.WriteEntry("PassiveClientService", ee.ToString(), EventLogEntryType.Error);
            }
        }

        private void serviceInstaller1_Committed(object sender, InstallEventArgs e)
        {
            try
            {
                using (ServiceController sc = new ServiceController(PassiveClientInstaller.ServiceName))
                {
                    SetRecoveryOptions(sc.ServiceName);
                }
            }
            catch (Exception e1)
            {
                EventLog.WriteEntry("PassiveClientService", e1.ToString(), EventLogEntryType.Error);
                return;
            }
        }

        static void SetRecoveryOptions(string serviceName)
        {
            int exitCode;
            using (var process = new Process())
            {
                var startInfo = process.StartInfo;
                startInfo.FileName = "sc";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                // tell Windows that the service should restart if it fails
                startInfo.Arguments = string.Format("failure \"{0}\" reset= 0 actions= restart/60000", serviceName);

                process.Start();
                process.WaitForExit();

                exitCode = process.ExitCode;
            }

            if (exitCode != 0)
                throw new InvalidOperationException();
        }
    }


}
