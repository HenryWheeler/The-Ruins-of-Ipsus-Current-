namespace The_Ruins_of_Ipsus
{
    class AIActions
    {
        public static void TestHuntAction(Entity AI)
        {
            AI detail = CMath.ReturnAI(AI);
            if (detail.target != null)
            {
                Vector2 positionToMove = DijkstraMaps.PathFromMap(AI, detail.target.GetComponent<Faction>().faction);
                Traversable traversable = World.tiles[positionToMove.x, positionToMove.y];
                if (traversable.actorLayer != null && detail.hatedEntities.Contains(traversable.actorLayer.GetComponent<Faction>().faction))
                {
                    AttackManager.MeleeAllStrike(AI, traversable.actorLayer);
                }
                else
                {
                    AI.GetComponent<Movement>().Move(new Vector2(positionToMove.x, positionToMove.y));
                    detail.interest--;

                    //List<OnUseProperty> abilities = detail.GrabAbilities(AI);
                    //if (abilities.Count == 0) { }
                }
            }
            else
            {
                Vector2 current = AI.GetComponent<Vector2>();
                Vector2 newPosition = new Vector2(World.random.Next(-1, 2) + current.x, World.random.Next(-1, 2) + current.y);
                if (CMath.CheckBounds(newPosition.x, newPosition.y))
                {
                    AI.GetComponent<Movement>().Move(newPosition);
                    detail.interest--;
                }
                else
                {
                    AI.GetComponent<TurnFunction>().EndTurn();
                }
            }
        }
        public static void TestPatrol(Entity AI)
        {
            PatrolFunction patrol = AI.GetComponent<PatrolFunction>();
            patrol.lastPastInformation--;
            Vector2 positionToMove = DijkstraMaps.PathFromMap(AI, $"Patrol{patrol.patrolRoute}");
            AI.GetComponent<Movement>().Move(new Vector2(positionToMove.x, positionToMove.y));
            if (positionToMove.x == patrol.lastPosition.x && positionToMove.y == patrol.lastPosition.y)
            {
                patrol.patrolRoute = World.seed.Next(0, 20);
            }
            else if (World.tiles[positionToMove.x, positionToMove.y].actorLayer != null && World.tiles[positionToMove.x, positionToMove.y].actorLayer != AI && CMath.ReturnAI(World.tiles[positionToMove.x, positionToMove.y].actorLayer) != null)
            {
                if (CMath.ReturnAI(AI).hatedEntities.Count != 0 && CMath.ReturnAI(AI).favoredEntities.Contains(World.tiles[positionToMove.x, positionToMove.y].actorLayer.GetComponent<Faction>().faction))
                {
                    foreach (string hated in CMath.ReturnAI(AI).hatedEntities)
                    {
                        CMath.ReturnAI(World.tiles[positionToMove.x, positionToMove.y].actorLayer).hatedEntities.Add(hated);
                    }
                    Log.Add("Someone spoke hatred");
                }
            }
            patrol.lastPosition = positionToMove;
        }
        public static void TestSleep(Entity AI)
        {
            AI detail = CMath.ReturnAI(AI);
            detail.interest--;

            if (detail.interest <= 0)
            {
                detail.interest = detail.baseInterest;
                detail.currentInput = The_Ruins_of_Ipsus.AI.Input.Noise;
            }
            AI.GetComponent<TurnFunction>().EndTurn();
        }
        public static void TestMimicWait(Entity AI)
        {
            AI detail = CMath.ReturnAI(AI);

            if (!AI.GetComponent<Mimicry>().disguised)
            {
                if (!AI.GetComponent<Mimicry>().CaptureGuise())
                {
                    if (DijkstraMaps.maps.ContainsKey("Obstacles"))
                    {
                        Vector2 vector2 = DijkstraMaps.PathFromMap(AI, "Obstacles");
                        AI.GetComponent<Movement>().Move(vector2);
                    }
                    else if (DijkstraMaps.maps.ContainsKey("Items"))
                    {
                        Vector2 vector2 = DijkstraMaps.PathFromMap(AI, "Items");
                        AI.GetComponent<Movement>().Move(vector2);
                    }
                    else
                    {
                        AI.GetComponent<TurnFunction>().EndTurn();
                    }
                }
                AI.GetComponent<TurnFunction>().EndTurn();
            }
            else
            {
                detail.interest--;
                if (detail.interest <= 0)
                {
                    Vector2 vector2 = AI.GetComponent<Vector2>();
                    AI.GetComponent<Movement>().Move(new Vector2(vector2.x + World.random.Next(-1, 2), vector2.y + World.random.Next(-1, 2)));
                    AI.GetComponent<Mimicry>().disguised = false;
                    detail.interest = detail.baseInterest;
                }
            }
        }

        public static void EngageEnemy(Entity AI)
        {
            AI detail = CMath.ReturnAI(AI);
            Entity target = detail.target;

            if (target == null)
            {
                detail.interest = 0;
                detail.Process();
                return;
            }
            else
            {
                int distanceToTarget = CMath.Distance(AI.GetComponent<Vector2>(), target.GetComponent<Vector2>());

                Log.Add($"Distance is {distanceToTarget}");

                if (distanceToTarget > detail.maxDistance)
                {
                    //Move to Target 
                    MoveToTarget(AI, target);

                    Log.Add($"Moves Forward 1");
                }
                else if (distanceToTarget < detail.minDistance)
                {
                    //Move away from Target
                    MoveToTarget(AI, target);

                    Log.Add($"Moves Away 1");
                }
                else
                {
                    //Engage target in combat
                    //Randomly determine whether AI moves towards or preferred distance or not, modify based on AI interest
                    //Randomly determine whether AI uses special ability or not
                    //AI will prioritize abilities with shorter range if the AI can use them
                    if (distanceToTarget <= 1)
                    {
                        if (!AttackCheck(AI, target, distanceToTarget))
                        {
                            AI.GetComponent<TurnFunction>().EndTurn();
                        }
                    }
                    else if (distanceToTarget > detail.preferredDistance || detail.preferredDistance < distanceToTarget)
                    {
                        if (World.random.Next(1, 101) + detail.interest > detail.baseInterest)
                        {
                            if (distanceToTarget > detail.preferredDistance)
                            {
                                //Move to Target 
                                MoveToTarget(AI, target);
                            }
                            else
                            {
                                //Move away from Target
                                MoveToTarget(AI, target);
                            }
                        }
                        else
                        {
                            if (!AttackCheck(AI, target, distanceToTarget))
                            {
                                if (distanceToTarget > detail.preferredDistance)
                                {
                                    //Move to Target 
                                    MoveToTarget(AI, target);
                                }
                                else
                                {
                                    //Move away from Target
                                    MoveToTarget(AI, target);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!AttackCheck(AI, target, distanceToTarget))
                        {
                            AI.GetComponent<TurnFunction>().EndTurn();
                        }
                    }
                }
            }
        }
        public static void MoveToTarget(Entity AI, Entity target)
        {
            if (target.GetComponent<Faction>() != null)
            {
                Vector2 positionToMove = DijkstraMaps.PathFromMap(AI, target.GetComponent<Faction>().faction);
                AI.GetComponent<Movement>().Move(new Vector2(positionToMove.x, positionToMove.y));
                CMath.ReturnAI(AI).interest--;
            }
            else { AI.GetComponent<TurnFunction>().EndTurn(); }
        }
        ///<summary>
        ///Check if AI has any special attacks it can use on its target, if not try for a melee attack, if that fails return false otherwise return true.
        ///</summary>
        public static bool AttackCheck(Entity AI, Entity target, int distance)
        {
            AI detail = CMath.ReturnAI(AI);

            if (World.random.Next(0, 101) < detail.abilityChance && AI.GetComponent<Usable>() != null)
            {
                OnUse abilityToUse = null;
                foreach (OnUse ability in AI.GetComponent<Usable>().onUseComponents)
                {
                    if (ability.itemType == "Offense" && ability.range >= distance)
                    {
                        if (abilityToUse == null) { abilityToUse = ability; }
                        else if (abilityToUse.range > abilityToUse.range) { abilityToUse = ability; }
                    }
                }
                UseAbility(abilityToUse, AI, target);
                return true;
            }
            else if (distance <= 1)
            {
                AttackManager.MeleeAllStrike(AI, target);

                return true;
            }

            return false;
        }
        public static void UseAbility(OnUse ability, Entity AI, Entity target)
        {
            ability.Use(AI, target.GetComponent<Vector2>());
            AI.GetComponent<TurnFunction>().EndTurn();

            if (ability.singleUse)
            {
                AI.RemoveComponent(ability);
            }
        }
        public static void TestAwake(Entity AI)
        {
            AI detail = CMath.ReturnAI(AI);
            detail.interest--;

            if (detail.interest <= 0)
            {
                detail.interest = detail.baseInterest;
                detail.currentInput = The_Ruins_of_Ipsus.AI.Input.Tired;
            }
            AI.GetComponent<TurnFunction>().EndTurn();
        }
    }
}
