using PassiveClient.Helpers.Shell.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Shell.Commands
{
    public class Copy : IShellCommand
    {
        private string _command;

        public bool IsMatch(string command)
        {
            _command = command;
            return command.ToLower().StartsWith("copy");
        }

        public string PerformCommand()
        {
            var args = _command.Split(' ').Skip(1).ToArray();
            if (args.Length < 2) throw new ArgumentException($"The copy command need at least 2 argumenrs");

            var copyFrom = args[0];
            var copyTo = args[1];

            File.Copy(copyFrom, copyTo);

            return Directory.GetCurrentDirectory();
        }
    }
}
