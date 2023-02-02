using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class Inventory: Component
    {
        public EquipmentSlot[] equipment { get; set; }
        public List<Entity> inventory = new List<Entity>();
        public EquipmentSlot ReturnSlot(string slotName)
        {
            foreach (EquipmentSlot slot in equipment)
            {
                if (slot != null && slot.name == slotName)
                {
                    return slot;
                }
            }
            return null;
        }
        public Inventory()
        {
            equipment = new EquipmentSlot[4];
            equipment[0] = new EquipmentSlot("Armor");
            equipment[1] = new EquipmentSlot("Weapon");
            equipment[2] = new EquipmentSlot("Off Hand");
            equipment[3] = new EquipmentSlot("Magic Item");
        }
    }
    public class EquipmentSlot
    {
        public string name { get; set; }
        public Entity item { get; set; }
        public EquipmentSlot(string _name)
        {
            name = _name;
        }
        public EquipmentSlot() { }
    }
}
