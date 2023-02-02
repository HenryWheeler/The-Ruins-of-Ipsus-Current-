using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace The_Ruins_of_Ipsus
{
    class SpawnTableManager
    {
        public static Dictionary<string, SpawnTable> spawnTables = new Dictionary<string, SpawnTable>();
        public SpawnTableManager()
        {
            foreach (SpawnTable table in JsonDataManager.PullAllTables())
            { spawnTables.Add(table.name, table); }
        }
        public static int RetrieveRandomEntity(string table, bool useSeed)
        {
            if (spawnTables.ContainsKey(table))
            {
                if (useSeed) { return spawnTables[table].table[World.seed.Next(1, spawnTables[table].table.Count + 1)]; }
                else { return spawnTables[table].table[World.random.Next(1, spawnTables[table].table.Count + 1)]; }
            }
            else { return 0; }
        }
    }
    [Serializable]
    public class SpawnTable
    {
        public string name { get; set; }
        public Dictionary<int, int> table = new Dictionary<int, int>();
        public SpawnTable(string _name) { name = _name; }
        public SpawnTable() { }
    }
}
