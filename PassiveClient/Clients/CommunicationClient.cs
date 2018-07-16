using PassiveClient.Clients;
using PassiveShell;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using System;
using System.ServiceModel;

namespace PassiveClient
{
    [Log(AttributeTargetElements = MulticastTargets.Method, AttributeTargetTypeAttributes = MulticastAttributes.Public, AttributeTargetMemberAttributes = MulticastAttributes.Private | MulticastAttributes.Public)]
    public class CommunicationClient : BaseClient
    {
        protected IPassiveShell _shelService;
        protected CallBack callback = null;
        protected StatusCallBack status = null;
        protected string _wcfServicesPathId;

        protected object initializeServiceReferences<T>(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = string.Format("PassiveShell/{0}", _wcfServicesPathId);
            }
            //Confuguring the Shell service
            var shellBinding = new BasicHttpBinding();
            shellBinding.Security.Mode = BasicHttpSecurityMode.None;
            shellBinding.CloseTimeout = TimeSpan.MaxValue;
            shellBinding.ReceiveTimeout = TimeSpan.MaxValue;
            shellBinding.SendTimeout = new TimeSpan(0, 0, 10, 0, 0);
            shellBinding.OpenTimeout = TimeSpan.MaxValue;
            shellBinding.MaxReceivedMessageSize = int.MaxValue;
            shellBinding.MaxBufferPoolSize = int.MaxValue;
            shellBinding.MaxBufferSize = int.MaxValue;
            //Put Public ip of the server copmuter
            var shellAdress = string.Format("http://localhost:80/ShellTrasferServer/{0}", path);
            var shellUri = new Uri(shellAdress);
            var shellEndpointAddress = new EndpointAddress(shellUri);
            var shellChannel = new ChannelFactory<T>(shellBinding, shellEndpointAddress);
            var shelService = shellChannel.CreateChannel();
            return shelService;
        }

        protected void RegisterCallBackCommunicationClient(Action<Shell> callBackFunctionCheckMission, Action onContinuationError)
        {
            var succeed = false;
            while (!succeed)
            {
                try
                {
                    status = new StatusCallBack();
                    callback = new CallBack();
                    callback.SetStatusCallbackAndCallbackCtorArgs(status, new CallbackCtorArgs()
                    {
                        CheckMission = callBackFunctionCheckMission,
                        ContinuationError = onContinuationError
                    });

                    status.SendServerCallBack(_wcfServicesPathId, id.ToString());
                    callback.SendServerCallBack(_wcfServicesPathId, id.ToString());
                    succeed = true;
                }
                catch
                {
                    if (callback != null)
                        callback.Dispose();
                    if (status != null)
                        status.Dispose();
                    Console.WriteLine("Error Register CallBacks, Disposing and trying again");
                }
            }
        }
    }
}
