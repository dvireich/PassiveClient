using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PassiveClient
{
    [Log(AttributeTargetElements = MulticastTargets.Method, AttributeTargetTypeAttributes = MulticastAttributes.Public, AttributeTargetMemberAttributes = MulticastAttributes.Private | MulticastAttributes.Public)]
    public sealed class Shell
    {
        private string _lastCommand;
        private Process _process;

        public void CloseShell()
        {
            if (_process != null)
            {
                _process.StandardOutput.Close();
                _process.StandardInput.Close();
                _process.Close();
            }
        }

        public string NextCommand(string command)
        {
            _lastCommand = command;
            var stdin = _process.StandardInput;
            stdin.WriteLine(command);
            StringBuilder str;
            string returnAns;
            GetStdOutString(stdin, out str, out returnAns, command);
            str.Clear();
            return returnAns;
        }

        [Log(AttributeExclude = true)]
        public bool WaitforExitAndAbort(Action act, int timeout)
        {
            var wait = new ManualResetEvent(false);
            var work = new Thread(() =>
            {
                act();
                wait.Set();
            });
            work.Start();
            var signal = wait.WaitOne(timeout);
            if (!signal)
            {
                work.Abort();
            }
            return signal;
        }

        private void GetStdOutString(StreamWriter stdin, out StringBuilder str, out string returnAns, string command)
        {
            var clientNextLine = "";
            stdin.WriteLine("echo #WAITING");
            stdin.Flush();

            var stdout = _process.StandardOutput;
            str = new StringBuilder();
            while (true)
            {
                string line = string.Empty;
                if (!WaitforExitAndAbort(() =>
                {
                    line = stdout.ReadLine();
                }, 30 * 1000))
                {
                    if (line.Contains("All"))
                    {
                        stdin.WriteLine("All");
                        stdin.WriteLine("echo #WAITING");
                    }
                    else if (line.Contains("Yes"))
                    {
                        stdin.WriteLine("Yes");
                        stdin.WriteLine("echo #WAITING");

                    }
                    else
                    {
                        break;
                    }

                }

                if (line == null)
                {
                    str.AppendLine("Error using command " + command);
                    break;
                }
                //The last line of the PClient command
                if (line == "Wating for command")
                {
                    str.AppendLine(line);
                    break;
                }
                if (line == "#WAITING")
                {
                    break;
                }
                if (line.Contains("#WAITING"))
                {
                    clientNextLine = line.Substring(0, line.Length - "echo#WAITING".Length - "\n\r".Length);
                }
                else
                {
                    str.AppendLine(line);
                }

            }
            str.AppendLine(clientNextLine);
            returnAns = str.ToString();
        }

        public string Run()
        {
            if (_process != null)
                _process.Close();
            var p = Process.Start(new ProcessStartInfo()
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                FileName = "cmd.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(@"C:\"),
                Verb = "runas"
            });
            _process = p;
            StringBuilder str;
            string returnAns;
            GetStdOutString(p.StandardInput, out str, out returnAns, "Activate");
            var resultString = str.ToString();
            str.Clear();
            return resultString;

        }
    }
}
