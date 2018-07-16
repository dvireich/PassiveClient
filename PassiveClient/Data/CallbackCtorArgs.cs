using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient
{
    [Log(AttributeExclude = true)]
    public class CallbackCtorArgs
    {
       public Action<Shell> CheckMission;
       public Action ContinuationError;
    }
}
