using System;
using System.Collections.Generic;
using System.Text;
using Queueker.core.Settings;

namespace Queueker.core.Transport
{
    public class TransportFactory
    {
        public static ITransport GetTransport(TransportTypes type, QueuekerSettings settings)
        {
            return type switch
            {
                TransportTypes.Http => (ITransport) new HttpTransport(settings),
                TransportTypes.Tcp => new TcpTransport(),
                _ => new HttpTransport(settings)
            };
        }
    }
}
