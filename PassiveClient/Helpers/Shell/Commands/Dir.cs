using PassiveClient.Helpers.Shell.Interfaces;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Shell.Commands
{
    class Dir : IShellCommand
    {
        private IDirHelper _dirHeleper;

        public Dir(IDirHelper dirHelper)
        {
            _dirHeleper = dirHelper;
        }

        [Log(AttributeExclude = true)]
        public bool IsMatch(string command)
        {
            return command.ToLower().Equals("dir");
        }

        public string PerformCommand()
        {
            return _dirHeleper.GenerateFilesAndDirString();
        }
    }
}
