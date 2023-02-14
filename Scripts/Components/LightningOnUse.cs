using System;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class LightningOnUse : OnUse
    {
        public override void Use(Entity entity, Vector2 target = null)
        {
            if (entity.GetComponent<PlayerComponent>() != null)
            {
                if (target == null)
                {
                    TargetReticle.StartTargeting(true, false);
                    TargetReticle.targetWeapon = this.entity;
                }
                else
                {
                    this.entity.GetComponent<Usable>().DisplayMessage(entity);
                    SpecialEffectManager.Lightning(entity, target, strength, range);
                    entity.GetComponent<TurnFunction>().EndTurn();
                }
            }
            else
            {
                this.entity.GetComponent<Usable>().DisplayMessage(entity);
                SpecialEffectManager.Lightning(entity, target, strength, range);
                entity.GetComponent<TurnFunction>().EndTurn();
            }
        }
        public LightningOnUse(int _strength, int _range, bool _singleUse = true)
        {
            strength = _strength;
            range = _range;
            singleUse = _singleUse;
            rangeModel = "Line";
            itemType = "Offense";
        }
        public LightningOnUse() { }
    }
}
