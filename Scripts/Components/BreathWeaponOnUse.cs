using System;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class BreathWeaponOnUse : OnUse
    {
        public string type { get; set; }
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
                    SpecialEffectManager.BreathWeapon(entity, target, strength, range, type);
                    entity.GetComponent<TurnFunction>().EndTurn();
                }
            }
            else
            {
                if (range == 0 || target == null)
                {
                    SpecialEffectManager.BreathWeapon(entity, entity.GetComponent<Vector2>(), strength, range, type);
                }
                else
                {
                    this.entity.GetComponent<Usable>().DisplayMessage(entity);
                    SpecialEffectManager.BreathWeapon(entity, target, strength, range, type);
                    entity.GetComponent<TurnFunction>().EndTurn();
                }
            }
        }
        public BreathWeaponOnUse(int _strength, string _type, int _range, bool _singleUse = true)
        {
            strength = _strength;
            range = _range;
            type = _type;
            itemType = "Offense";
            singleUse = _singleUse;
            rangeModel = "Cone";
        }
        public BreathWeaponOnUse() { }
    }
}
