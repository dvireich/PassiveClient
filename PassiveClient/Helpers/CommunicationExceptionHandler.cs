using PassiveClient.Helpers;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient
{
    [Log(AttributeExclude = true)]
    public class CommunicationExceptionHandler : ICommunicationExceptionHandler
    {
        [Log(AttributeExclude = true)]
        public void SendRequestAndTryAgainIfTimeOutOrEndpointNotFound(Action op, Action inTimeOutException = null, Action inCommunicationException = null, Action inGeneralException = null)
        {
            while (true)
            {
                try
                {
                    op();
                    break;
                }
                catch (System.TimeoutException)
                {
                    if (inTimeOutException != null)
                        inTimeOutException();
                    //try again
                }
                catch (CommunicationException)
                {
                    if (inCommunicationException != null)
                        inCommunicationException();
                    //try again
                }
                catch (Exception e)
                {
                    if (inGeneralException != null)
                        inGeneralException();
                    else
                        throw e;
                }
            }
        }

        [Log(AttributeExclude = true)]
        public T SendRequestAndTryAgainIfTimeOutOrEndpointNotFound<T>(Func<T> op, Action inTimeOutException = null, Action inCommunicationException = null, Action inGeneralException = null)
        {
            while (true)
            {
                try
                {
                    return op();
                }
                catch (TimeoutException)
                {
                    if (inTimeOutException != null)
                        inTimeOutException();
                    //try again
                }
                catch (CommunicationException)
                {
                    if (inCommunicationException != null)
                        inCommunicationException();
                    //try again
                }
                catch (Exception e)
                {
                    if (inGeneralException != null)
                        inGeneralException();
                    else
                        throw e;
                }
            }
        }
    }
}
