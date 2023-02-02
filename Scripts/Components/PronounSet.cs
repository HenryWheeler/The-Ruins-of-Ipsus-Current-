using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class PronounSet: Component
    {
        public string subjective { get; set; }
        public string objective { get; set; }
        public string possesive { get; set; }
        public string reflexive { get; set; }
        public bool present { get; set; }
        public PronounSet(string _sub, string _obj, string _pos, string _ref, bool _present = true)
        {
            subjective = _sub; objective = _obj;
            possesive = _pos; reflexive = _ref;
            present = _present;
        }
        public PronounSet() { }
    }
}
