using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Queueker.core.Protocol;

namespace Queueker.core.Transport
{
    public class TcpTransport : ITransport
    {
        public Task<string> SendToServer(MessageInfo messageInfo)
        {
            throw new NotImplementedException();
        }
    }
}
