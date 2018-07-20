using AlertCallBack;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Callback_Implementation
{

    [Log(AttributeTargetElements = MulticastTargets.Method, AttributeTargetTypeAttributes = MulticastAttributes.Public, AttributeTargetMemberAttributes = MulticastAttributes.Private | MulticastAttributes.Public)]
    public abstract class BaseCallBack : IAletCallBackCallback, IDisposable
    {
        public static AletCallBackClient proxy;
        private bool isDead;
        public string _id;

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

        public void SendServerCallBack(string wcfServicesPathId, string id, string callbackType)
        {
            _id = id;
            Uri endPointAdress = new Uri(string.Format("net.tcp://localhost/ShellTrasferServer/CallBack/{0}", wcfServicesPathId));
            NetTcpBinding wsd = new NetTcpBinding();
            wsd.Security.Mode = SecurityMode.None;
            wsd.CloseTimeout = TimeSpan.MaxValue;
            wsd.ReceiveTimeout = TimeSpan.MaxValue;
            wsd.OpenTimeout = TimeSpan.MaxValue;
            wsd.SendTimeout = TimeSpan.MaxValue;

            //SetReliableSessionAndInactivityTimeoutAndReaderQuotas(wsd);

            EndpointAddress ea = new EndpointAddress(endPointAdress);
            proxy = new AletCallBackClient(new InstanceContext(this), wsd, ea);
            proxy.InnerDuplexChannel.OperationTimeout = TimeSpan.MaxValue;
            proxy.InnerChannel.OperationTimeout = TimeSpan.MaxValue;
            proxy.RegisterCallBackFunction(id, callbackType);
        }

        public virtual void CallBackFunction(string str) { }

        public void KeppAlive()
        {
            if (!isDead)
                proxy.KeepCallBackAlive(_id);
        }

        public void KeepCallbackALive()
        {
            //Do nothing, this function is only for the server in order to send traffic through the connection pipe  
        }

        public void Dispose()
        {
            isDead = true;
            if (proxy != null && proxy.State == CommunicationState.Opened)
                proxy.Close();
        }
    }
}
