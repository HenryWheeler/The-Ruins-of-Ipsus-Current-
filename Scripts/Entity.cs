using System;
using System.Collections.Generic;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class Entity
    {
        public List<Component> collectionsToRemove = new List<Component>();
        public List<Component> components = new List<Component>();
        public void AddComponent(Component component)
        {
            if (component != null && !components.Contains(component))
            {
                components.Add(component);
                component.entity = this;

                if (component.GetType().BaseType.Equals(typeof(OnHit)))
                {
                    if (GetComponent<Harmable>() != null && !GetComponent<Harmable>().onHitComponents.Contains((OnHit)component))
                    {
                        GetComponent<Harmable>().onHitComponents.Add((OnHit)component);
                    }
                    else if (GetComponent<AttackFunction>() != null && !GetComponent<AttackFunction>().onHitComponents.Contains((OnHit)component))
                    {
                        GetComponent<AttackFunction>().onHitComponents.Add((OnHit)component);
                    }
                }
                else if (component.GetType().BaseType.Equals(typeof(OnMove)) && !GetComponent<Movement>().onMoveComponents.Contains((OnMove)component))
                {
                    GetComponent<Movement>().onMoveComponents.Add((OnMove)component);
                }
                else if (component.GetType().BaseType.Equals(typeof(OnThrow)) && !GetComponent<Throwable>().onThrowComponents.Contains((OnThrow)component))
                {
                    GetComponent<Throwable>().onThrowComponents.Add((OnThrow)component);
                }
                else if (component.GetType().BaseType.Equals(typeof(OnTurn)) && !GetComponent<TurnFunction>().onTurnComponents.Contains((OnTurn)component))
                {
                    GetComponent<TurnFunction>().onTurnComponents.Add((OnTurn)component);
                }
                else if (component.GetType().BaseType.Equals(typeof(OnUse)) && !GetComponent<Usable>().onUseComponents.Contains((OnUse)component))
                {
                    GetComponent<Usable>().onUseComponents.Add((OnUse)component);
                }
            }
        }
        public void RemoveComponent(Component component)
        {
            if (component != null)
            {
                components.Remove(component);
                component.entity = null;

                if (component.GetType().BaseType.Equals(typeof(OnHit)))
                {
                    if (GetComponent<Harmable>() != null)
                    {
                        GetComponent<Harmable>().onHitComponents.Remove((OnHit)component);
                    }
                    else if (GetComponent<AttackFunction>() != null)
                    {
                        GetComponent<AttackFunction>().onHitComponents.Remove((OnHit)component);
                    }
                }
                else if (component.GetType().BaseType.Equals(typeof(OnMove)))
                {
                    GetComponent<Movement>().onMoveComponents.Remove((OnMove)component);
                }
                else if (component.GetType().BaseType.Equals(typeof(OnThrow)))
                {
                    GetComponent<Throwable>().onThrowComponents.Remove((OnThrow)component);
                }
                else if (component.GetType().BaseType.Equals(typeof(OnTurn)))
                {
                    GetComponent<TurnFunction>().onTurnComponents.Remove((OnTurn)component);
                }
                else if (component.GetType().BaseType.Equals(typeof(OnUse)))
                {
                    GetComponent<Usable>().onUseComponents.Remove((OnUse)component);
                }
            }
        }
        public T GetComponent<T>() where T : Component
        {
            foreach (Component component in components)
            {
                if (component.GetType().Equals(typeof(T)))
                {
                    return (T)component;
                }
            }
            return null;
        }

        public void ClearCollections()
        {
            if (collectionsToRemove.Count != 0)
            {
                foreach (Component component in collectionsToRemove)
                {
                    RemoveComponent(component);
                }
                collectionsToRemove.Clear();
            }
        }
        public void ClearImbeddedComponents()
        {
            if (GetComponent<Harmable>() != null)
            {
                GetComponent<Harmable>().onHitComponents.Clear();
            }
            if (GetComponent<AttackFunction>() != null)
            {
                GetComponent<AttackFunction>().onHitComponents.Clear();
            }
            if (GetComponent<Movement>() != null)
            {
                GetComponent<Movement>().onMoveComponents.Clear();
            }
            if (GetComponent<Throwable>() != null)
            {
                GetComponent<Throwable>().onThrowComponents.Clear();
            }
            if (GetComponent<TurnFunction>() != null)
            {
                GetComponent<TurnFunction>().onTurnComponents.Clear();
            }
            if (GetComponent<Usable>() != null)
            {
                GetComponent<Usable>().onUseComponents.Clear();
            }
        }
        public void ClearEntity()
        {
            foreach (Component component in components)
            {
                if (component != null)
                {
                    component.entity = null;
                }
            }
            components = null;
        }
        public Entity(Entity entity)
        {
            List<Component> tempAdditons = new List<Component>();
            foreach (Component component in entity.components)
            {
                if (component != null)
                {
                    if (component.GetType().BaseType.Equals(typeof(TurnFunction)))
                    {
                        AddComponent((TurnFunction)component);
                        TurnManager.AddActor(entity.GetComponent<TurnFunction>());
                    }
                    else if (component.GetType().BaseType.Equals(typeof(OnHit)) || component.GetType().BaseType.Equals(typeof(OnMove)) || component.GetType().BaseType.Equals(typeof(OnThrow)) || component.GetType().BaseType.Equals(typeof(OnTurn)) || component.GetType().BaseType.Equals(typeof(OnUse)))
                    {
                        tempAdditons.Add(component);
                    }
                    else
                    {
                        AddComponent(component);
                    }
                }
            }

            foreach (Component component in tempAdditons)
            {
                if (component != null)
                {
                    AddComponent(component);
                }
            }
        }
        public Entity(List<Component> _components)
        {
            List<Component> tempAdditons = new List<Component>();
            foreach (Component component in _components)
            {
                if (component != null)
                {
                    if (component.GetType().Equals(typeof(TurnFunction)))
                    {
                        AddComponent((TurnFunction)component);
                        TurnManager.AddActor(GetComponent<TurnFunction>());
                    }
                    else if (component.GetType().BaseType.Equals(typeof(OnHit)) || component.GetType().BaseType.Equals(typeof(OnMove)) || component.GetType().BaseType.Equals(typeof(OnThrow)) || component.GetType().BaseType.Equals(typeof(OnTurn)) || component.GetType().BaseType.Equals(typeof(OnUse)))
                    {
                        tempAdditons.Add(component);
                    }
                    else
                    {
                        AddComponent(component);
                    }
                }
            }

            foreach (Component component in tempAdditons)
            {
                if (component != null)
                {
                    AddComponent(component);
                }
            }
        }
        public Entity() { }
    }
}
