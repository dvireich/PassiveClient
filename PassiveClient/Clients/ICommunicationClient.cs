﻿using PassiveShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Clients
{
    public interface ICommunicationClient
    {
        object InitializeServiceReferences<T>(string path = null);

        IPassiveShell ShelService { get; set; }
    }
}
