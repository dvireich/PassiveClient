using PassiveClient.Helpers.Shell.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Shell.Commands
{
    public class DirBareFolder : IShellCommand
    {
        public bool IsMatch(string command)
        {
            return command.ToLower().Equals("dir /b /ad");
        }

        public string PerformCommand()
        {
            return DirHelper.GenerateBareFormatFolderString();
        }
    }
}
