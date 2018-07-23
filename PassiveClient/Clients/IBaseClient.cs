using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Clients
{
    public interface IBaseClient
    {
        Guid Id { get; set; }
    }
}
