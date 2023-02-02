using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class Visibility: Component
    {
        public bool opaque { get; set; }
        public bool visible { get; set; }
        public bool explored { get; set; }
        public void SetVisible(bool visibility) { if (visibility) { visible = true; explored = true; } else { visible = false; } }
        public Visibility(bool _opaque, bool _visible, bool _explored) { opaque = _opaque; visible = _visible; explored = _explored; }
        public Visibility() { }
    }
}
