using System;
using System.Collections.Generic;
using System.Text;

namespace Queueker.core.Protocol
{
    public class ProtocolInnerMessage
    {
        public string subcommand { get; set; }
        public string message { get; set; }
    }
}
