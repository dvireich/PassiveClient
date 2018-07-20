using PassiveClient.Helpers.Shell.Interfaces;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Shell.Commands
{
    public class DirBareFormat : IShellCommand
    {
        private IDirHelper _dirHeleper;

        public DirBareFormat( IDirHelper dirHeleper)
        {
            _dirHeleper = dirHeleper;
        }

        [Log(AttributeExclude = true)]
        public bool IsMatch(string command)
        {
            return command.ToLower().Equals("dir /b");
        }

        public string PerformCommand()
        {
            return _dirHeleper.GenerateBareFormatAllFileAndFolderString();
        }
    }
}
