using System;
using System.Collections.Generic;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class Throwable : Component
    {
        public List<OnThrow> onThrowComponents = new List<OnThrow>();
        public bool consumable = true;
        public string throwMessage { get; set; }
        public void Throw(Entity user, Vector2 landingSite)
        {
            foreach (OnThrow component in onThrowComponents)
            {
                if (component != null)
                {
                    component.Throw(user, landingSite);
                }
            }
        }
        public Throwable(string _throwMessage)
        {
            throwMessage = _throwMessage;
        }
        public Throwable() { }
    }
}
