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
        public static TitleConsole console;
        private static string spacer { get; set; }
        public StatManager(TitleConsole _console)
        {
            console = _console;
            spacer = " + ";
        }
        public static void UpdateStats(Entity entity)
        {
            console.Clear();

            Stats stats = entity.GetComponent<Stats>();

            string display = "";

            display += $"Red*Health: {stats.hp}/{stats.hpCap}{spacer}";
            display += $"Light_Gray*Armor: {stats.ac}{spacer}";
            display += $"Yellow*Speed: {stats.maxAction}{spacer}";
            display += $"Brown*Might: {stats.strength}{spacer}";
            display += $"Cyan*Acuity: {stats.acuity}{spacer}";
            display += $"Sight: {stats.sight}{spacer}";

            display += $"Status:{spacer}";
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

            CMath.DisplayToConsole(console, display, 0, 2, 1);

            CMath.DisplayToConsole(console, $"Open Inventory Yellow*[I] {spacer}Open Equipment Yellow*[E]", 0, 2, 1, 29, false);

            Renderer.CreateConsoleBorder(console, false);

            console.Print(2, 0, $" Stats {(char)196} ", Color.White);
            console.Print(11, 0, $"Equipment ", Color.Gray);
            console.Print(21, 0, $"{(char)196}", Color.White);
            console.Print(22, 0, $" Inventory ", Color.Gray);

        }
    }
}
