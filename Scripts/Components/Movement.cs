using System;
using System.Collections.Generic;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class Movement : Component
    {
        public List<OnMove> onMoveComponents = new List<OnMove>();
        public List<int> moveTypes = new List<int>();
        public void Move(Vector2 newPosition)
        {
            if (entity.GetComponent<Harmable>().statusEffects.Contains("Restrained"))
            {
                PronounSet pronouns = entity.GetComponent<PronounSet>();
                if (World.random.Next(1, 21) + ((entity.GetComponent<Stats>().strength - 10) / 2) > 10)
                {
                    if (pronouns.present)
                    {
                        Log.Add($"{entity.GetComponent<Description>().name} has freed {pronouns.reflexive} from {pronouns.possesive} restraints.");
                    }
                    else
                    {
                        Log.Add($"{entity.GetComponent<Description>().name} have freed {pronouns.reflexive} from {pronouns.possesive} restraints.");
                    }
                    entity.GetComponent<Harmable>().statusEffects.Remove("Restrained");
                }
                else
                {
                    if (pronouns.present)
                    {
                        Log.Add($"{entity.GetComponent<Description>().name} struggles in {pronouns.possesive} restraints.");
                    }
                    else
                    {
                        Log.Add($"{entity.GetComponent<Description>().name} struggle in {pronouns.possesive} restraints.");
                    }
                }
                entity.GetComponent<TurnFunction>().EndTurn();
            }
            else
            {
                Vector2 originalPosition = entity.GetComponent<Vector2>();
                Traversable newTraversable = World.tiles[newPosition.x, newPosition.y];
                if (CMath.CheckBounds(newPosition.x, newPosition.y) && moveTypes.Contains(newTraversable.terrainType))
                {
                    if (newTraversable.actorLayer == null)
                    {
                        World.tiles[originalPosition.x, originalPosition.y].actorLayer = null;
                        entity.GetComponent<Vector2>().x = newPosition.x;
                        entity.GetComponent<Vector2>().y = newPosition.y;
                        newTraversable.actorLayer = entity;
                        TriggerOnMove(originalPosition, newPosition);
                        if (newTraversable.obstacleLayer != null && newTraversable.obstacleLayer.GetComponent<Movement>() != null)
                        {
                            newTraversable.obstacleLayer.GetComponent<Movement>().TriggerOnMove(originalPosition, newPosition);
                        }
                        entity.GetComponent<TurnFunction>().EndTurn();
                        EntityManager.UpdateMap(entity);
                    }
                    else if (CMath.ReturnAI(newTraversable.actorLayer) != null && !CMath.ReturnAI(newTraversable.actorLayer).hatedEntities.Contains(entity.GetComponent<Faction>().faction))
                    {
                        World.tiles[originalPosition.x, originalPosition.y].actorLayer = newTraversable.actorLayer;
                        newTraversable.actorLayer.GetComponent<Vector2>().x = entity.GetComponent<Vector2>().x;
                        newTraversable.actorLayer.GetComponent<Vector2>().y = entity.GetComponent<Vector2>().y;
                        EntityManager.UpdateMap(newTraversable.actorLayer);
                        entity.GetComponent<Vector2>().x = newPosition.x;
                        entity.GetComponent<Vector2>().y = newPosition.y;
                        newTraversable.actorLayer = entity;
                        TriggerOnMove(originalPosition, newPosition);
                        if (newTraversable.obstacleLayer != null && newTraversable.obstacleLayer.GetComponent<Movement>() != null)
                        {
                            newTraversable.obstacleLayer.GetComponent<Movement>().TriggerOnMove(originalPosition, newPosition);
                        }
                        entity.GetComponent<TurnFunction>().EndTurn();
                        EntityManager.UpdateMap(entity);
                    }
                    else if (entity.GetComponent<PlayerComponent>() != null)
                    {
                        AttackManager.MeleeAllStrike(entity, newTraversable.actorLayer);
                    }
                    else
                    {
                        entity.GetComponent<TurnFunction>().EndTurn();
                    }
                }
                else if (entity.GetComponent<PlayerComponent>() != null)
                {
                    Log.Add("You cannot move there.");
                    Log.DisplayLog();
                }
                else
                {
                    entity.GetComponent<TurnFunction>().EndTurn();
                }
            }
        }
        public void TriggerOnMove(Vector2 initialPosition, Vector2 finalPosition)
        {
            foreach (OnMove component in onMoveComponents)
            {
                if (component != null)
                {
                    component.Move(initialPosition, finalPosition);
                }
            }
        }
        public Movement(List<int> _moveTypes)
        {
            moveTypes = _moveTypes;
        }
        public Movement() { }
    }
}
