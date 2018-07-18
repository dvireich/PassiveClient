using PassiveClient.Helpers.Shell.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Shell.Commands
{
    public class Cd : IShellCommand
    {
        private string _command;

        public bool IsMatch(string command)
        {
            _command = command.ToLower(); ;

            return command.ToLower().StartsWith("cd") &&
                   (string.IsNullOrEmpty(command.ToLower().Substring(0, "cd..".Length)) || 
                    string.IsNullOrEmpty(command.ToLower().Substring(0, "cd ..".Length)));
        }

        public string PerformCommand()
        {
            string path = string.Empty;
            if(_command == "cd.." || _command == "cd ..")
            {
                path = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            }
            else
            {
                var arg = string.Join(" ",_command.Split(' ').Skip(1));
                path = Directory.Exists(arg) ? arg : Path.Combine(Directory.GetCurrentDirectory(), arg);
            }
            Directory.SetCurrentDirectory(path);

            return Directory.GetCurrentDirectory();
        }
    }
}
