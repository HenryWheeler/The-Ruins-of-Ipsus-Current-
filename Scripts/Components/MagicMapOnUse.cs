using System;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class MagicMapOnUse : OnUse
    {
        public override void Use(Entity entity, Vector2 target = null)
        {
            SpecialEffectManager.MagicMap(entity);
        }
        public MagicMapOnUse()
        {
            strength = 0;
            singleUse = true;
            itemType = "Support";
            rangeModel = "All";
        }
    }
}
