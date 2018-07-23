using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Clients
{
    public class BaseClient : IBaseClient
    {
        public BaseClient()
        {
            Id = Guid.NewGuid();
        }

        public BaseClient(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
