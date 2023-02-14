using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    class ID: Component
    {
        public string entityType { get; set; }
        public int temporaryID = 0;
        public string ReturnInstanceReference()
        {
            return $"{entity.GetComponent<Description>().name}-{temporaryID}";
        }
        public ID(string type)
        {
            entityType = type;
        }
        public ID() { }
    }
}
