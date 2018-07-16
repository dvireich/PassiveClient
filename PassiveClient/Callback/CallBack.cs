using AlertCallBack;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Linq;
using System.ServiceModel;
using static PassiveClient.PassiveClient;

namespace PassiveClient
{
    [Log(AttributeTargetElements = MulticastTargets.Method, AttributeTargetTypeAttributes = MulticastAttributes.Public, AttributeTargetMemberAttributes = MulticastAttributes.Private | MulticastAttributes.Public)]
    public class CallBack : IAletCallBackCallback, IDisposable
    {
        private AletCallBackClient _proxy;
        private CallbackCtorArgs _callbackCtorArgs;
        private Shell _shellHandler;
        private StatusCallBack _statusCallBack;
        private bool _isDead = false;
        private string _id;

        private void SetReliableSessionAndInactivityTimeoutAndReaderQuotas(NetTcpBinding wsd)
        {
            wsd.ReliableSession.Enabled = true;
            wsd.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
            System.Xml.XmlDictionaryReaderQuotas readerQuotas = new System.Xml.XmlDictionaryReaderQuotas();
            readerQuotas.MaxDepth = System.Int32.MaxValue;
            readerQuotas.MaxStringContentLength = System.Int32.MaxValue;
            readerQuotas.MaxArrayLength = System.Int32.MaxValue;
            readerQuotas.MaxBytesPerRead = System.Int32.MaxValue;
            readerQuotas.MaxNameTableCharCount = System.Int32.MaxValue;
            wsd.ReaderQuotas = readerQuotas;
        }

        public void SetStatusCallbackAndCallbackCtorArgs(StatusCallBack status, CallbackCtorArgs callbackCtorArgs)
        {
            _statusCallBack = status;
            _callbackCtorArgs = callbackCtorArgs;
            _shellHandler = new Shell();
        }

        public void SendServerCallBack(string wcfServicesPathId, string id)
        {
            _id = id;

            Uri endPointAdress = new Uri(string.Format("net.tcp://localhost/ShellTrasferServer/CallBack/{0}", wcfServicesPathId));
            NetTcpBinding wsd = new NetTcpBinding();
            wsd.Security.Mode = SecurityMode.None;
            wsd.CloseTimeout = TimeSpan.MaxValue;
            wsd.ReceiveTimeout = TimeSpan.MaxValue;
            wsd.OpenTimeout = TimeSpan.MaxValue;
            wsd.SendTimeout = TimeSpan.MaxValue;
            EndpointAddress ea = new EndpointAddress(endPointAdress);
            _proxy = new AletCallBackClient(new InstanceContext(this), wsd, ea);
            _proxy.InnerDuplexChannel.OperationTimeout = TimeSpan.MaxValue;
            _proxy.InnerChannel.OperationTimeout = TimeSpan.MaxValue;
            _proxy.RegisterCallBackFunction(id, "Main");
        }

        public void CallBackFunction(string str)
        {
            if (str == "livnessCheck")
                return;
            else if (str.Split(' ').First().ToLower() == "nickname")
                _passiveClientNickName = str.Split(' ').Last();
            try
            {
                _callbackCtorArgs.CheckMission(_shellHandler);
            }
            catch
            {
                Dispose();
                _statusCallBack.Dispose();
                _callbackCtorArgs.ContinuationError();
            }
        }

        public void Dispose()
        {
            try
            {
                _isDead = true;
                if (_proxy != null && _proxy.State == CommunicationState.Opened)
                    _proxy.Close();
            }
            catch { }
            
        }

        public void KeppAlive()
        {
            if (!_isDead)
                _proxy.KeepCallBackAlive(_id);
        }

        public void KeepCallbackALive()
        {
            //Do nothing, this function is only for the server in order to send traffic through the connection pipe  
        }
    }
}
