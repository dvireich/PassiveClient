using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Data
{
    [Log(AttributeExclude = true)]
    public class HardDrive
    {
        public string Name { get; set; }

        public string Letter { get; set; }

        public string Model { get; set; }

        public string Type { get; set; }

        public string SerialNo { get; set; }
    }
}
