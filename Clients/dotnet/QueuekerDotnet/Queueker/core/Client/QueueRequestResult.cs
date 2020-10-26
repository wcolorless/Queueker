using System;
using System.Collections.Generic;
using System.Text;

namespace Queueker.core.Client
{
    public class QueueRequestResult<T>
    {
        public bool Complete { get; set; }
        public T Result { get; set; }
    }
}
