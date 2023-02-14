using SadRogue.Primitives;
using SadConsole;
using System.Collections.Generic;
using System.Linq;

namespace The_Ruins_of_Ipsus
{
    public class Log
    {
        private static Queue<string> log = new Queue<string>();
        private static int maxLogCount = 15;
        private static string lastLog { get; set; }
        private static int repeatCount = 1;
        private static string spacer { get; set; }
        public Log()
        {
            spacer = " + ";
            for (int i = 0; i < maxLogCount; i++)
            {
                log.Enqueue("");
            }
        }
        public static void DisplayLog()
        {
            Program.logConsole.Clear();

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
                            if (c + split[0].Length > Program.logConsole.Width - 5) { y += 2 + m; c = 1; }
                            Program.logConsole.Print(c + 1, y, split[0], Color.White);
                            c += split[0].Length + 1;
                        }
                    }
                    else
                    {
                        if (split[1].Contains("+")) { y += 2 + m; c = 1; }
                        else
                        {
                            if (c + split[0].Length > Program.logConsole.Width - 5) { y += 2 + m; c = 1; }
                            Program.logConsole.Print(c + 1, y, split[1], ColorFinder.ColorPicker(split[0]));
                            c += split[1].Length + 1;
                        }
                    }
                }
            }
            Renderer.CreateConsoleBorder(Program.logConsole, true);
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
                                    new ParticleComponent(World.random.Next(12, 15), "North", 2, new Draw[] { new Draw(color, "Black", characters[i]) })
                                });
                    Renderer.AddParticle(firstX, position.y, particle);
                }

                firstX++;
            }
        }
        public static void Add(string logAdd)
        {
            if (logAdd == lastLog)
            {
                repeatCount++;

                string[] temp = log.ToArray();
                log.Clear();
                temp[temp.Length - 1] = $"{spacer}{logAdd} Yellow*x{repeatCount}";

                foreach (string s in temp)
                {
                    log.Enqueue(s);
                }

                if (log.Count > maxLogCount)
                {
                    log.Dequeue();
                }
            }
            else
            {
                repeatCount = 1;
                log.Enqueue(spacer + logAdd);
                if (log.Count > maxLogCount)
                {
                    log.Dequeue();
                }
            }
            lastLog = logAdd;
        }
    }
}
