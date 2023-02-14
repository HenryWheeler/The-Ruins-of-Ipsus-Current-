using System;
using SadConsole;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    public class StatManager
    {
        private static string spacer { get; set; }
        public StatManager()
        {
            spacer = " + ";
        }
        public static void UpdateStats(Entity entity)
        {
            Program.playerConsole.Clear();

            Stats stats = entity.GetComponent<Stats>();

            string display = "";

            display += $"Red*Health: {stats.hp}/{stats.hpCap}{spacer}";
            display += $"Light_Gray*Armor: {stats.ac}{spacer}";
            display += $"Yellow*Speed: {stats.maxAction}{spacer}";
            display += $"Brown*Might: {stats.strength}{spacer}";
            display += $"Cyan*Acuity: {stats.acuity}{spacer}";
            display += $"Sight: {stats.sight}{spacer}";

            display += $"Status: {spacer}";
            for (int i = 0; i < entity.GetComponent<Harmable>().statusEffects.Count; i++)
            {
                if (i == entity.GetComponent<Harmable>().statusEffects.Count - 1)
                {
                    display += $"{entity.GetComponent<Harmable>().statusEffects[i]}.";
                }
                else
                {
                    display += $"{entity.GetComponent<Harmable>().statusEffects[i]}, ";
                }
            }

            if (World.developerMode)
            {
                display += $"Debug Information: {spacer}";
                display += $"Current Actor Count = {EntityManager.actorCount} {spacer}";
                display += $"Current Item Count = {EntityManager.itemCount} {spacer}";
                display += $"Current Obstacle Count = {EntityManager.obstacleCount} {spacer}";
            }

            CMath.DisplayToConsole(Program.playerConsole, display, 0, 2, 1);

            Renderer.CreateConsoleBorder(Program.playerConsole, true);
        }
    }
}
