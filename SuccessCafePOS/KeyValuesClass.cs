using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuccessCafePOS
{
    public class KeyValuesClass
    {
        public byte Header { get; set; }

        public byte Footer { get; set; }

        public byte ProductCode { get; set; }

        public Edition Edition { get; set; }

        public byte Version { get; set; }

        public byte Type { get; set; }

        public DateTime Expiration { get; set; }
    }
}
