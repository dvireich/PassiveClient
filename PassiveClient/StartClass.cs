using PassiveClient;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Backends.Log4Net;
using System;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

[Log(AttributeExclude = true)]
public class StartClass
{

    public static void Main(string[] args)
    {
        InitializeLoggingBackend();

        Program.StartFunc(args);

    }

    public static void InitializeLoggingBackend()
    {
        log4net.Config.XmlConfigurator.Configure();
        var log4NetLoggingBackend = new Log4NetLoggingBackend();
        LoggingServices.DefaultBackend = log4NetLoggingBackend;
    }
}