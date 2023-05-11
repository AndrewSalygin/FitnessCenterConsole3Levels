using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessCenterConsole.Common {
    class StructuresData {
        [Serializable]
        public struct KeyValuePair<K, V> {
            public K Key { get; set; }
            public V Value { get; set; }
        }
    }
}
