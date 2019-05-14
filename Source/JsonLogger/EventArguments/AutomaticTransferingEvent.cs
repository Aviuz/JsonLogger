using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonLogger.EventArguments
{
    public class AutomaticTransferingEvent
    {
        public string FilePath { get; set; }

        public bool Cancel { get; set; }
    }
}
