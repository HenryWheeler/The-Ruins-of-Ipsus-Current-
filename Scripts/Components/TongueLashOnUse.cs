using System;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    class TongueLashOnUse : OnUse
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
                    SpecialEffectManager.TongueLash(entity, target, strength, range);
                    entity.GetComponent<TurnFunction>().EndTurn();
                }
            }
        }
        public TongueLashOnUse(int _strength, int _range)
        {
            strength = _strength;
            range = _range;
            rangeModel = "Line";
        }
        public TongueLashOnUse() { }
    }
}
