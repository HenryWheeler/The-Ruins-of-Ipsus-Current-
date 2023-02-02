using System;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class Equippable : Component
    {
        public bool equipped { get; set; }
        public string slot { get; set; }
        public bool unequipable { get; set; }
        public int ac { get; set; }
        public void Equip(Entity entityRef)
        {
            entityRef.GetComponent<Inventory>().ReturnSlot(slot).item = entity;
            equipped = true;

            entityRef.GetComponent<Stats>().ac += ac;
            if (entityRef.GetComponent<PlayerComponent>() != null) { StatManager.UpdateStats(entityRef); }
        }
        public void Unequip(Entity entityRef)
        {
            entityRef.GetComponent<Inventory>().ReturnSlot(slot).item = null;
            equipped = false;

            entityRef.GetComponent<Stats>().ac -= ac;
            if (entityRef.GetComponent<PlayerComponent>() != null) { StatManager.UpdateStats(entityRef); }
        }
        public Equippable(string _slot, bool _unequipable) { equipped = false; slot = _slot; unequipable = _unequipable; }
        public Equippable() { }
    }
}