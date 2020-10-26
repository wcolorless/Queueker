using System;
using System.Collections.Generic;
using System.Text;

namespace Queueker.core.Protocol
{
    public class QueueItemInfo
    {
        public string Name { get; set; }
        public long Limit { get; set; }
        public long Items { get; set; }
    }
}
