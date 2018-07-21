using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Interfaces
{
    public interface IFileInfoHelper
    {
        long GetFileLength(string path);
    }
}
