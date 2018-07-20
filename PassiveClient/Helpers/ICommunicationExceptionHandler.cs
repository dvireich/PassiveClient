using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers
{
    public interface ICommunicationExceptionHandler
    {
         T SendRequestAndTryAgainIfTimeOutOrEndpointNotFound<T>(Func<T> op, Action inTimeOutException = null, Action inCommunicationException = null, Action inGeneralException = null);

        void SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(Action op, Action inTimeOutException = null, Action inCommunicationException = null, Action inGeneralException = null);
    }
}
