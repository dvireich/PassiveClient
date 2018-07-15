using PassiveClient.ServiceReference1;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using static PassiveClient.Program;

namespace PassiveClient
{
    [Log(AttributeTargetElements = MulticastTargets.Method, AttributeTargetTypeAttributes = MulticastAttributes.Public, AttributeTargetMemberAttributes = MulticastAttributes.Private | MulticastAttributes.Public)]
    public class CallBack : IAletCallBackCallback, IDisposable
    {
        public static AletCallBackClient proxy;
        public static Shell shellHandler = new Shell();
        public static StatusCallBack statusCallBack;
        private bool isDead = false;

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

        public void SetStatusCallback(StatusCallBack status)
        {
            statusCallBack = status;
        }
        public void SendServerCallBack()
        {
            Uri endPointAdress = new Uri(string.Format("net.tcp://localhost/ShellTrasferServer/CallBack/{0}", _wcfServicesPathId));
            NetTcpBinding wsd = new NetTcpBinding();
            wsd.Security.Mode = SecurityMode.None;
            wsd.CloseTimeout = TimeSpan.MaxValue;
            wsd.ReceiveTimeout = TimeSpan.MaxValue;
            wsd.OpenTimeout = TimeSpan.MaxValue;
            wsd.SendTimeout = TimeSpan.MaxValue;
            EndpointAddress ea = new EndpointAddress(endPointAdress);
            proxy = new AletCallBackClient(new InstanceContext(this), wsd, ea);
            proxy.InnerDuplexChannel.OperationTimeout = TimeSpan.MaxValue;
            proxy.InnerChannel.OperationTimeout = TimeSpan.MaxValue;
            proxy.RegisterCallBackFunction(id.ToString(), "Main");
        }

        public void CallBackFunction(string str)
        {
            if (str == "livnessCheck")
                return;
            else if (str.Split(' ').First().ToLower() == "nickname")
                _passiveClientNickName = str.Split(' ').Last();
            try
            {
                CheckMissions(shellHandler);
            }
            catch (Exception e)
            {
                Dispose();
                statusCallBack.Dispose();
                if (shelService != null)
                {
                    ((ICommunicationObject)shelService).Close();
                    shelService = null;
                }
                MainLoop();
            }
        }

        public void Dispose()
        {
            try
            {
                isDead = true;
                if (proxy != null && proxy.State == CommunicationState.Opened)
                    proxy.Close();
            }
            catch { }
            
        }

        public void KeppAlive()
        {
            if (!isDead)
                proxy.KeepCallBackAlive(id.ToString());
        }

        public void KeepCallbackALive()
        {
            //Do nothing, this function is only for the server in order to send traffic through the connection pipe  
        }
    }
}
