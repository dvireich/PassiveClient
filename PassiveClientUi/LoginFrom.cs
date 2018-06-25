using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
                this.Visible = false;
                if (!PassiveClient.Program.Authenticate(UserNameTextBox.Text, PasswordTextBox.Text))
                {
                    MessageBox.Show("User Name or Password are incorrent", "Sign in", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Visible = true;
                    return;
                }
                PassiveClient.Program.Main(new string[]
                {
                    "hidden=True",
                    "morethanoneclinet=false",
                });
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Visible = true;
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
