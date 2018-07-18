using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Interfaces
{
    public interface IShell
    {
        string NextCommand(string command);

        string Run();

        void CloseShell();
    }
}
