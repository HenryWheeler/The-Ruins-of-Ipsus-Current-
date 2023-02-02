using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace The_Ruins_of_Ipsus
{
    public class TurnManager
    {
        public static List<TurnFunction> entities = new List<TurnFunction>();
        private static int turn = 0;
        private static int entityTurn = 0;
        public static bool threadRunning = false;
        public static void ProgressTurnOrder()
        {
            if (entities.Count != 0)
            {
                turn++;
                if (entityTurn >= entities.Count - 1) entityTurn = 0;
                else entityTurn++;
                if (entities[entityTurn].actionLeft <= 0) ProgressActorTurn(entities[entityTurn]);
                else entities[entityTurn].StartTurn();
            }
        }
        public static void ProgressActorTurn(TurnFunction entity)
        {
            if (entity.actionLeft <= 0) { entity.actionLeft += entity.entity.GetComponent<Stats>().maxAction; ProgressTurnOrder(); }
            else
            {
                entity.actionLeft--;
                if (entity.actionLeft <= 0) { entity.actionLeft += entity.entity.GetComponent<Stats>().maxAction; ProgressTurnOrder(); }
                else { entity.StartTurn(); }
            }
        }
        public static void AddActor(TurnFunction entity)
        {
            if (entity.entity != null && !entities.Contains(entity)) { entities.Add(entity); }
        }
        public static void RemoveActor(TurnFunction entity) { entities.Remove(entity); }
    }
}
