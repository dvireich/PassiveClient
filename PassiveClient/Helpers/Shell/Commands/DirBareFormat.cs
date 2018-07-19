using PassiveClient.Helpers.Shell.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Shell.Commands
{
    public class DirBareFormat : IShellCommand
    {
        public bool IsMatch(string command)
        {
            return command.ToLower().Equals("dir /b");
        }

        public string PerformCommand()
        {
            return DirHelper.GenerateBareFormatAllFileAndFolderString();
        }
    }
}
