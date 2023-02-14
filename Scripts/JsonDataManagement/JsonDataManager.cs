using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace The_Ruins_of_Ipsus.Scripts.JsonDataManagement
{
    public class JsonDataManager
    {
        private static JsonSerializerSettings options;
        private static string entityPath { get; set; }
        private static string tablePath { get; set; }
        public JsonDataManager()
        {
            options = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            entityPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "JsonData/EntityData");
            tablePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "JsonData/TableData");

            if (!Directory.Exists(entityPath)) Directory.CreateDirectory(entityPath);
            if (!Directory.Exists(tablePath)) Directory.CreateDirectory(tablePath);
        }
        public static List<Entity> PullAllEntities()
        {
            List<Entity> pullData = new List<Entity>();
            foreach (string filePath in Directory.GetFiles(entityPath)) { JsonConvert.DeserializeObject<Entity>(File.ReadAllText(filePath), options); }
            return pullData;
        }
        public static List<SpawnTable> PullAllTables()
        {
            List<SpawnTable> pullData = new List<SpawnTable>();
            foreach (string filePath in Directory.GetFiles(tablePath))
            {
                SpawnTable table = JsonConvert.DeserializeObject<SpawnTable>(File.ReadAllText(filePath), options);
                pullData.Add(table);
            }
            return pullData;
        }
        public static Entity ReturnEntity(string name)
        {
            string pullData = File.ReadAllText(Path.Combine(entityPath, name + ".json"));
            return new Entity(JsonConvert.DeserializeObject<Entity>(pullData, options));
        }
        public static void SaveEntity(Entity entity, string name)
        {
            entity.ClearImbeddedComponents();
            if (!Directory.Exists(entityPath)) Directory.CreateDirectory(entityPath);
            File.WriteAllText(Path.Combine(entityPath, name + ".json"), JsonConvert.SerializeObject(entity, options));
        }
        public static void SaveTable(SpawnTable table)
        {
            if (!Directory.Exists(tablePath)) Directory.CreateDirectory(tablePath);
            File.WriteAllText(Path.Combine(tablePath, table.name + ".json"), JsonConvert.SerializeObject(table, options));
        }
    }
}
