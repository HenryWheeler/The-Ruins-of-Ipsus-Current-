using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using The_Ruins_of_Ipsus.Scripts.JsonDataManagement;
using System.Threading;

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
        /// <summary>
        /// Give the passed entity some items, the number is determined by the creatures level.
        /// </summary>
        /// <param name="entity"></param>
        /// The entity to give items to.
        /// <param name="seeded"></param>
        /// Is the entity to be given items according to the seed or not.
        public static void GiveRandomItems(Entity entity, bool seeded) 
        {
            int level = entity.GetComponent<Stats>().level;
            int count;
            if (seeded)
            {
                count = World.seed.Next(1, level + 1);
            }
            else
            {
                count = World.random.Next(1, level + 1);
            }

            for (int i = 0; i < count; i++)
            {
                string itemName;
                if (seeded)
                {
                    itemName = RetrieveRandomEntity("Item_Table", level, true);
                }
                else 
                {
                    itemName = RetrieveRandomEntity("Item_Table", level, false);
                }
                InventoryManager.AddToInventory(entity, EntityManager.CreateEntityFromFile(new Vector2(0, 0), itemName, false));
            }
        }
        public static void CreateNewTable(string tableName, Dictionary<int, string> tableDictionary)
        {
            JsonDataManager.SaveTable(new SpawnTable(tableName, tableDictionary));
        }
        public static string RetrieveRandomEntity(string table, int modifier, bool useSeed)
        {
            if (spawnTables.ContainsKey(table))
            {
                if (modifier == 0) { modifier = 1; }
                if (useSeed) { return spawnTables[table].table[World.random.Next(1, 21) * modifier]; }
                else { return spawnTables[table].table[World.random.Next(1, 21) * modifier]; }
            }
            else { throw new Exception("Referenced table does not exist"); }
        }
    }
    [Serializable]
    public class SpawnTable
    {
        public string name { get; set; }
        public Dictionary<int, string> table = new Dictionary<int, string>();
        public SpawnTable(string name) { this.name = name; }
        public SpawnTable(string name, Dictionary<int, string> table) { this.name = name; this.table = table; }
        public SpawnTable() { }
    }
}
