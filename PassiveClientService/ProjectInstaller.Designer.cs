namespace PassiveClientService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PassiveClientProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.PassiveClientInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // PassiveClientProcessInstaller
            // 
            this.PassiveClientProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.PassiveClientProcessInstaller.Password = null;
            this.PassiveClientProcessInstaller.Username = null;
            // 
            // PassiveClientInstaller
            // 
            this.PassiveClientInstaller.Description = "This is a Passive Client the allow to Active client with the same user name and p" +
    "assword to transfer files with this PC";
            this.PassiveClientInstaller.DisplayName = "Passive Client Service";
            this.PassiveClientInstaller.ServiceName = "Dvir";
            this.PassiveClientInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.PassiveClientInstaller.Committed += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller1_Committed);
            this.PassiveClientInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller1_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.PassiveClientProcessInstaller,
            this.PassiveClientInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller PassiveClientProcessInstaller;
        private System.ServiceProcess.ServiceInstaller PassiveClientInstaller;
    }
}