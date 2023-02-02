using SadRogue.Primitives;
using SadConsole;
using System.Collections.Generic;
using System.Linq;

namespace The_Ruins_of_Ipsus
{
    public class Log
    {
        public static TitleConsole console;
        private static Queue<string> log = new Queue<string>();
        private static int maxLogCount = 15;
        private static string spacer { get; set; }
        public Log(TitleConsole _console)
        {
            console = _console;
            spacer = " + ";
            for (int i = 0; i < maxLogCount; i++)
            {
                log.Enqueue("");
            }
        }
        public static void DisplayLog()
        {
            console.Clear();

            int m = 0;
            int y = 0;
            int c = 1;

            string[] temp = log.ToArray<string>();
            //temp.Reverse();

            for (int i = temp.Length - 1; i >= 0; i--)
            {
                string[] outPut = temp[i].Split(' ');

                foreach (string text in outPut)
                {
                    string[] split = text.Split('*');
                    if (split.Count() == 1)
                    {
                        if (split[0].Contains("+")) { y += 2 + m; c = 1; }
                        else
                        {
                            if (c + split[0].Length > console.Width - 5) { y += 2 + m; c = 1; }
                            console.Print(c + 1, y, split[0], Color.White);
                            c += split[0].Length + 1;
                        }
                    }
                    else
                    {
                        if (split[1].Contains("+")) { y += 2 + m; c = 1; }
                        else
                        {
                            if (c + split[0].Length > console.Width - 5) { y += 2 + m; c = 1; }
                            console.Print(c + 1, y, split[1], ColorFinder.ColorPicker(split[0]));
                            c += split[1].Length + 1;
                        }
                    }
                }
            }
            Renderer.CreateConsoleBorder(console, true);

        }
        public static void OutputParticleLog(string log, string color, Vector2 position)
        {
            string name = "";

            foreach (string text in log.Split(' '))
            {
                string[] split = text.Split('*');
                if (split.Count() == 1)
                {
                    name += split[0] + " ";
                }
                else
                {
                    name += split[1] + " ";
                }
            }

            char[] characters = name.ToCharArray();
            int firstX = position.x - characters.Length / 2;

            for (int i = 0; i < characters.Length; i++)
            {
                if (CMath.CheckBounds(firstX, position.y))
                {
                    Entity particle = new Entity(new List<Component>
                                {
                                    new Vector2(0, 0),
                                    new Draw(color, "Black", characters[i]),
                                    new ParticleComponent(World.random.Next(9, 12), 2, "North", 1, new Draw[] { new Draw(color, "Black", characters[i]) })
                                });
                    Renderer.AddParticle(firstX, position.y, particle);
                }

                firstX++;
            }
        }
        public static void AddToStoredLog(string logAdd)
        {
            string newString = spacer + logAdd;
            log.Enqueue(newString);
            if (log.Count > maxLogCount)
            {
                log.Dequeue();
            }
        }
        public static void Add(string logAdd)
        {
            log.Enqueue(spacer + logAdd);
            if (log.Count > maxLogCount)
            {
                log.Dequeue();
            }
        }
    }
}
