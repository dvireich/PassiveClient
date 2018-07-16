using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient
{
    [Log(AttributeExclude = true)]
    public static class Constants
    {
        public const string virsion = "11";
        public const string run = "Run";
        public const string nextCommand = "NextCommand";
        public const string closeShell = "CloseShell";
        public const string download = "Download";
        public const string upload = "Upload";
    }
}
