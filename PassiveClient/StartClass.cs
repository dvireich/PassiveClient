using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Backends.Log4Net;

namespace PassiveClient
{
    [Log(AttributeExclude = true)]
    public class StartClass
    {

        public static void Main(string[] args)
        {
            InitializeLoggingBackend();
            var passiveClient = new PassiveClient();
            passiveClient.Main(args);
        }

        public static void InitializeLoggingBackend()
        {
            log4net.Config.XmlConfigurator.Configure();
            var log4NetLoggingBackend = new Log4NetLoggingBackend();
            LoggingServices.DefaultBackend = log4NetLoggingBackend;
        }
    }
}

