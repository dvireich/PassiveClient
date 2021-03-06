﻿using PassiveClient.Helpers.Shell.Interfaces;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Shell.Commands
{
    public class DirBareFolder : IShellCommand
    {
        private IDirHelper _dirHeleper;

        public DirBareFolder(IDirHelper dirHelper)
        {
            _dirHeleper = dirHelper;
        }

        [Log(AttributeExclude = true)]
        public bool IsMatch(string command)
        {
            return command.ToLower().Equals("dir /b /ad");
        }

        public string PerformCommand()
        {
            return _dirHeleper.GenerateBareFormatFolderString();
        }
    }
}
