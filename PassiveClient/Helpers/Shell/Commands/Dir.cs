﻿using PassiveClient.Helpers.Shell.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Shell.Commands
{
    class Dir : IShellCommand
    {
        public bool IsMatch(string command)
        {
            return command.ToLower().StartsWith("dir") &&
                   string.IsNullOrEmpty(command.ToLower().Substring(0, "dir".Length));
        }

        public string PerformCommand()
        {
            return DirHelper.GenerateFilesAndDirString();
        }
    }
}
