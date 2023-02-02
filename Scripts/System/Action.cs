using SadConsole.Input;
using System.Linq;

namespace The_Ruins_of_Ipsus
{
    public class Action
    {
        /*
        private static bool actorPresent = false;
        private static bool itemPresent = false;
        private static bool obstaclePresent = false;
        private static bool terrainPresent = false;
        private static string interactionText = "";
        private static string interactionKeyText = "";
        */
        public static bool choosingDirection = false;
        public static bool choosingTarget = false;
        public static bool interacting = false;
        public static void EscapeInteraction(Entity player)
        {
            player.GetComponent<TurnFunction>().turnActive = true;
            interacting = false;
            choosingDirection = false;
            choosingTarget = false;
            StatManager.UpdateStats(player);
            Log.DisplayLog();
        }
        public static void ChooseDirection(Entity player, int x, int y)
        {
            /*
            Vector2 vector2 = player.GetComponent<Vector2>();
            chosenLocation = new Vector2(vector2.x + x, vector2.y + y);
            Traversable traversable = World.tiles[chosenLocation.x, chosenLocation.y];
            interactionKeyText = "";
            int interactions = 0;
            if (x != 0 || y != 0)
            {
                if (traversable.actorLayer != null)
                {
                    actorPresent = true;
                    interactionKeyText += traversable.actorLayer.GetComponent<Description>().name + ":[E] + ";
                    interactions++;
                }
                else { actorPresent = false; }
            }
            else { actorPresent = false; }
            if (traversable.itemLayer != null)
            {
                itemPresent = true;
                interactionKeyText += traversable.itemLayer.GetComponent<Description>().name + ":[I] + ";
                interactions++;
            }
            else { itemPresent = false; }
            if (traversable.obstacleLayer != null)
            {
                obstaclePresent = true;
                interactionKeyText += traversable.obstacleLayer.GetComponent<Description>().name + ":[O] + ";
                interactions++;
            }
            else { obstaclePresent = false; }
            //if (traversable.entity.GetComponent<Interactable>() != null)
            {
                terrainPresent = true;
                interactionKeyText += traversable.entity.GetComponent<Description>().name + ":[T] + ";
                interactions++;
            }
            else { terrainPresent = false; }
            //if (!actorPresent && !itemPresent && !obstaclePresent && !terrainPresent)
            //{ interactionText = "There is nothing there."; CMath.DisplayToConsole(Log.console, interactionText, 1, 1); }
            //else if (interactions == 1)
            {
                choosingDirection = false;
                if (actorPresent) { ChooseEntity(player, 0); }
                else if (itemPresent) { ChooseEntity(player, 1); }
                else if (obstaclePresent) { ChooseEntity(player, 2); }
                else if (terrainPresent) { ChooseEntity(player, 3); }
            }
            //else
            {
                interactionText = "What do you interact with?";
                interactionKeyText += "End Interaction:[Escape]";
                choosingDirection = false;
                choosingTarget = true;
                Interaction(player);
            }
            */
        }
        public static void DisplayActions(string actions)
        {
            //CMath.ClearConsole(console); CMath.DisplayToConsole(console, actions, 0, 2); console.Print(8, 0, " Actions ", RLColor.White); 
        }
    }
}
