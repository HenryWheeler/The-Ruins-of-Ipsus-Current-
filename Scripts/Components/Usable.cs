using System;
using System.Collections.Generic;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class Usable : Component
    {
        public List<OnUse> onUseComponents = new List<OnUse>();
        public string useMessage { get; set; }
        public bool autoTarget { get; set; }
        public void Use(Entity user, Vector2 target)
        {
            if (autoTarget)
            {
                DisplayMessage(user);
            }

            foreach (OnUse component in onUseComponents)
            {
                if (component != null)
                {
                    component.Use(user, target);
                }
            }
        }
        public void DisplayMessage(Entity user)
        {
            if (user != entity)
            {
                if (user.GetComponent<PronounSet>().present)
                {
                    Log.Add(user.GetComponent<Description>().name + " has used the " + entity.GetComponent<Description>().name + "! " + useMessage);
                }
                else
                {
                    Log.Add(user.GetComponent<Description>().name + " have used the " + entity.GetComponent<Description>().name + "! " + useMessage);
                }
            }
            else
            {
                if (user.GetComponent<PronounSet>().present)
                {
                    Log.Add(user.GetComponent<Description>().name + " has " + useMessage);
                }
                else
                {
                    Log.Add(user.GetComponent<Description>().name + " have " + useMessage);
                }
            }
        }
        public Usable(string _useMessage, bool _autoTarget = true)
        {
            useMessage = _useMessage;
            autoTarget = _autoTarget;
        }
        public Usable() { }
    }
}
