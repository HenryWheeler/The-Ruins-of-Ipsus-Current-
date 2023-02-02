namespace The_Ruins_of_Ipsus
{
    class ExplodeOnUse : OnUse
    {
        public override void Use(Entity entity, Vector2 target = null)
        {
            if (entity.GetComponent<PlayerComponent>() != null)
            {
                if (range == 0)
                {
                    SpecialEffectManager.Explosion(entity, entity.GetComponent<Vector2>(), strength);
                }
                else if (target == null)
                {
                    TargetReticle.StartTargeting(true, false);
                    TargetReticle.targetWeapon = this.entity;
                }
                else
                {
                    this.entity.GetComponent<Usable>().DisplayMessage(entity);
                    SpecialEffectManager.Explosion(entity, target, strength);
                    entity.GetComponent<TurnFunction>().EndTurn();
                }
            }
            else
            {
                if (range == 0 || target == null)
                {
                    SpecialEffectManager.Explosion(entity, entity.GetComponent<Vector2>(), strength);
                }
                else
                {
                    this.entity.GetComponent<Usable>().DisplayMessage(entity);
                    SpecialEffectManager.Explosion(entity, target, strength);
                    entity.GetComponent<TurnFunction>().EndTurn();
                }
            }
        }
        public ExplodeOnUse(int _strength, int _range, bool _singleUse = true)
        {
            strength = _strength;
            range = _range;
            singleUse = _singleUse;
            itemType = "Offense";
            rangeModel = "Sphere";
        }
        public ExplodeOnUse() { }
    }
    class ExplodeOnThrow : OnThrow
    {
        public override void Throw(Entity user, Vector2 landingSite)
        {
            SpecialEffectManager.Explosion(entity, landingSite, strength);
        }
        public ExplodeOnThrow(int _strength)
        {
            strength = _strength;
            rangeModel = "Sphere";
            itemType = "Offense";
        }
        public ExplodeOnThrow() { }
    }
}
