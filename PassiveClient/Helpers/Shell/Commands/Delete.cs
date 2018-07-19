using PassiveClient.Helpers.Shell.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Shell.Commands
{
    public class Delete : IShellCommand
    {
        private string _command;

        public bool IsMatch(string command)
        {
            _command = command;
            return command.StartsWith("del");
        }

        public string PerformCommand()
        {
            var pathToDel = _command.Split(' ').Skip(1).FirstOrDefault();
            if (pathToDel == null) throw new ArgumentNullException($"The command should not get null argument: {_command} ");
                                 
            var path = string.Empty;                                              
            if(File.Exists(pathToDel))
            {
                path = pathToDel;
            }
            else if(File.Exists(Path.Combine(Directory.GetCurrentDirectory(), pathToDel)))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), pathToDel);
            }
            else if (Directory.Exists(pathToDel))
            {
                path = pathToDel;
            }
            else if(Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), pathToDel)))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), pathToDel);
            }

            File.Delete(path);

            return Directory.GetCurrentDirectory();
        }
    }
}
