using System;
using System.Collections.Generic;
using System.Text;

namespace Queueker.core.Protocol
{
    public class MessageInfo
    {
        public string login { get; set; }
        public string pass { get; set; }
        public string direct { get; set; }
        public string quename { get; set; }
        public string command { get; set; }
        public List<byte> data { get; set; }
    }
}
