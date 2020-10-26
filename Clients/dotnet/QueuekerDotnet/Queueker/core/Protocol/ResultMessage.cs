using System;
using System.Collections.Generic;
using System.Text;

namespace Queueker.core.Protocol
{
    public class ResultMessage
    {
        public string Result { get; set; }
        public string Message { get; set; }
        public List<byte> Data { get; set; }
    }
}
