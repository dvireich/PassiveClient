using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PassiveClientUi
{
    public partial class LoginFrom : Form
    {
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
                if (!PassiveClient.Program.Authenticate(UserNameTextBox.Text, PasswordTextBox.Text))
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
                    Process.Start(new ProcessStartInfo()
                    {
                        UseShellExecute = false,
                        FileName = "PassiveClient.exe",
                        Arguments = string.Join(" ", new string[] {
                                                                    "hidden=true",
                                                                    "morethanoneclinet=false",
                                                                     string.Format("username={0}",UserNameTextBox.Text),
                                                                     string.Format("password={0}",PasswordTextBox.Text)}),
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    });
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
