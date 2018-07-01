using PassiveClientUi.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PassiveClientUi
{
    public partial class LoginFrom : Form
    {
        const string _userType = "PassiveClient";
        public LoginFrom()
        {
            InitializeComponent();
        }

        private void EnterKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AuthenticateAndStart();
            }
        }

        private void AuthenticateAndStart()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (!Authenticate(UserNameTextBox.Text, PasswordTextBox.Text))
                {
                    MessageBox.Show("User Name or Password are incorrent", "Sign in", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var messageForSuccess = string.Concat("Successfully Signed In!",
                                                      Environment.NewLine,
                                                      "Wating in background for Active user to sign in...");

                MessageBox.Show(messageForSuccess, "Sign in", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Visible = false;
                Cursor.Current = Cursors.Default;

                try
                {
                    StartAsProcess();
                }
                finally
                {
                    Environment.Exit(0);
                }
            }
            catch(Exception e)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(e.Message , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void StartAsProcess()
        {
            var argsArr = new List<string> {"hidden=true",
                                            "morethanoneclinet=true",
                                             string.Format("username={0}",UserNameTextBox.Text),
                                             string.Format("password={0}",PasswordTextBox.Text)};
            if (AddToServices.Checked)
            {
                argsArr.Add("/Install");
            }
            Process.Start(new ProcessStartInfo()
            {
                UseShellExecute = false,
                FileName = "PassiveClient.exe",
                Arguments = string.Join(" ", argsArr),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            });
        }

        private bool Authenticate(string userName, string password)
        {
            var auth = (IAuthentication)initializeServiceReferences<IAuthentication>("Authentication");
            var resp = auth.AuthenticateAndSignIn(new AuthenticateAndSignInRequest()
            {
                userName = userName,
                password = password,
                userType = _userType
            });
            //We do not want the server add this passiveClient yet, when we will open the PassiveClient.exe then 
            //He will signin. 
            //So we will logout now
            auth.Logout(new LogoutRequest()
            {
                userName = userName,
                userType = _userType
            });
            if (auth != null)
            {
                ((ICommunicationObject)auth).Close();
            }
            return !string.IsNullOrEmpty(resp.AuthenticateAndSignInResult);
        }

        private object initializeServiceReferences<T>(string path)
        {
            //Confuguring the Shell service
            var shellBinding = new BasicHttpBinding();
            shellBinding.Security.Mode = BasicHttpSecurityMode.None;
            shellBinding.CloseTimeout = TimeSpan.MaxValue;
            shellBinding.ReceiveTimeout = TimeSpan.MaxValue;
            shellBinding.SendTimeout = new TimeSpan(0, 0, 10, 0, 0);
            shellBinding.OpenTimeout = TimeSpan.MaxValue;
            shellBinding.MaxReceivedMessageSize = int.MaxValue;
            shellBinding.MaxBufferPoolSize = int.MaxValue;
            shellBinding.MaxBufferSize = int.MaxValue;
            //Put Public ip of the server copmuter
            var shellAdress = string.Format("http://localhost:80/ShellTrasferServer/{0}", path);
            var shellUri = new Uri(shellAdress);
            var shellEndpointAddress = new EndpointAddress(shellUri);
            var shellChannel = new ChannelFactory<T>(shellBinding, shellEndpointAddress);
            var shelService = shellChannel.CreateChannel();
            return shelService;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            AuthenticateAndStart();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
