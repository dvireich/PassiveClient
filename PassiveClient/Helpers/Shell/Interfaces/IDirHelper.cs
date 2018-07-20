using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Shell.Interfaces
{
    public interface IDirHelper
    {
        string GenerateBareFormatAllFileAndFolderString();

        string GenerateBareFormatFolderString();

        string GenerateFilesAndDirString();
    }
}
