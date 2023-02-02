using System.Collections.Generic;

namespace The_Ruins_of_Ipsus
{
    public class EntityManager
    {
        public static Dictionary<string, List<Entity>> actors = new Dictionary<string, List<Entity>>();
        public static List<Entity> items = new List<Entity>();
        public static List<Entity> obstacles = new List<Entity>();
        public static Dictionary<int, Entity> entityReferences = new Dictionary<int, Entity>();
        public static void UpdateAll()
        {
            if (actors.Count != 0)
            {
                foreach (List<Entity> entities in actors.Values)
                {
                    UpdateMap(entities[0]);
                }
            }
            if (items.Count != 0)
            {
                UpdateMap(items[0]);
            }
            if (obstacles.Count != 0)
            {
                UpdateMap(obstacles[0]);
            }
        }
        public static void LoadAllEntities()
        {
            List<Entity> entities = JsonDataManager.PullAllEntities();
            foreach (Entity entity in entities)
            {
                if (entity != null)
                {
                    entityReferences.Add(entity.GetComponent<ID>().id, entity);
                }
            }
        }
        public static void ClearAllIndexes()
        {
            foreach (List<Entity> entities in actors.Values)
            {
                foreach (Entity actor in entities)
                {
                    if (actor != null && actor.GetComponent<TurnFunction>() != null && actor.GetComponent<ID>().id != 0)
                    { TurnManager.RemoveActor(actor.GetComponent<TurnFunction>()); }
                }
            }
            actors.Clear();
            items.Clear();
            obstacles.Clear();
        }
        public static void AddEntity(Entity entity, bool update = true)
        {
            if (entity != null)
            {
                int id = entity.GetComponent<ID>().id;
                if (id <= 1000)
                {
                    string faction = entity.GetComponent<Faction>().faction;
                    if (!actors.ContainsKey(faction))
                    {
                        actors.Add(faction, new List<Entity>());
                    }
                    actors[faction].Add(entity);
                }
                else if (id > 1000 && id <= 2000)
                {
                    items.Add(entity);
                }
                else
                {
                    obstacles.Add(entity);
                }

                if (update)
                {
                    UpdateMap(entity);
                }
            }
        }
        public static void RemoveEntity(Entity entity, bool update = true)
        {
            int id = entity.GetComponent<ID>().id;
            if (id <= 1000)
            {
                string faction = entity.GetComponent<Faction>().faction;
                actors[faction].Remove(entity);
                if (actors[faction].Count == 0)
                {
                    actors.Remove(faction);
                    DijkstraMaps.maps.Remove(faction);
                }
                else
                {
                    UpdateMap(entity);
                }
            }
            else if (id > 1000 && id <= 2000)
            {
                items.Remove(entity);
                if (items.Count == 0)
                {
                    DijkstraMaps.maps.Remove("Items");
                }
                else
                {
                    UpdateMap(entity);
                }
            }
            else
            {
                obstacles.Remove(entity);
                if (obstacles.Count == 0)
                {
                    DijkstraMaps.maps.Remove("Obstacles");
                }
                else
                {
                    UpdateMap(entity);
                }
            }
        }
        public static void UpdateMap(Entity entity)
        {
            int id = entity.GetComponent<ID>().id;
            if (id >= 0 && id <= 1000)
            {
                string faction = entity.GetComponent<Faction>().faction;
                DijkstraMaps.CreateMap(actors[faction], faction);
            }
            else if (id > 1000 && id <= 2000) { DijkstraMaps.CreateMap(items, "Items"); }
            else { DijkstraMaps.CreateMap(obstacles, "Obstacles"); }
        }
        public static void ClearAIOfEntity(Entity entity)
        {
            foreach (List<Entity> entities in actors.Values)
            {
                foreach (Entity actor in entities)
                {
                    if (actor != null && CMath.ReturnAI(actor) != null && CMath.ReturnAI(actor).target == entity)
                    {
                        CMath.ReturnAI(actor).target = null;
                    }
                }
            }
        }
        public static Entity CreateEntity(Vector2 vector3, int uID, bool random, bool seeded = false)
        {
            Entity entity = JsonDataManager.ReturnEntity(uID);

            if (CMath.ReturnAI(entity) != null)
            {
                CMath.ReturnAI(entity).SetTransitions();
            }

            if (random)
            {
                vector3.x = 0; vector3.y = 0;
                bool sufficient = false;
                while (!sufficient)
                {
                    if (seeded)
                    {
                        vector3.x = World.seed.Next(0, Program.gameMapWidth);
                        vector3.y = World.seed.Next(0, Program.gameMapHeight);
                    }
                    else
                    {
                        vector3.x = World.random.Next(0, Program.gameMapWidth);
                        vector3.y = World.random.Next(0, Program.gameMapHeight);
                    }
                    if (CMath.CheckBounds(vector3.x, vector3.y))
                    {
                        if (entity.GetComponent<Movement>() != null)
                        {
                            if (entity.GetComponent<Movement>().moveTypes.Contains(World.tiles[vector3.x, vector3.y].terrainType))
                            { sufficient = true; }
                        }
                        else
                        {
                            if (World.tiles[vector3.x, vector3.y].terrainType != 0)
                            { sufficient = true; }
                        }
                    }
                }
            }

            if (seeded)
            {
                AddEntity(entity, false);
            }
            else
            {
                AddEntity(entity);
            }

            entity.GetComponent<Vector2>().x = vector3.x;
            entity.GetComponent<Vector2>().y = vector3.y;
            int id = entity.GetComponent<ID>().id;
            if (id <= 1000) { World.tiles[vector3.x, vector3.y].actorLayer = entity; }
            else if (id > 1000 && id <= 2000) { World.tiles[vector3.x, vector3.y].itemLayer = entity; }
            else { World.tiles[vector3.x, vector3.y].obstacleLayer = entity; }
            return entity;
        }
        public static Entity CreateEntity(Vector2 vector3, Entity entity, bool random, bool seeded = false)
        {
            if (CMath.ReturnAI(entity) != null)
            {
                CMath.ReturnAI(entity).SetTransitions();
            }

            if (random)
            {
                vector3.x = 0; vector3.y = 0;
                bool sufficient = false;
                while (!sufficient)
                {
                    if (seeded)
                    {
                        vector3.x = World.seed.Next(0, Program.gameMapWidth);
                        vector3.y = World.seed.Next(0, Program.gameMapHeight);
                    }
                    else
                    {
                        vector3.x = World.random.Next(0, Program.gameMapWidth);
                        vector3.y = World.random.Next(0, Program.gameMapHeight);
                    }
                    if (CMath.CheckBounds(vector3.x, vector3.y))
                    {
                        if (entity.GetComponent<Movement>() != null)
                        {
                            if (entity.GetComponent<Movement>().moveTypes.Contains(World.tiles[vector3.x, vector3.y].terrainType))
                            { sufficient = true; }
                        }
                        else
                        {
                            if (World.tiles[vector3.x, vector3.y].terrainType != 0)
                            { sufficient = true; }
                        }
                    }
                }
            }
            AddEntity(entity);
            entity.GetComponent<Vector2>().x = vector3.x;
            entity.GetComponent<Vector2>().y = vector3.y;
            int id = entity.GetComponent<ID>().id;
            if (id <= 1000) { World.tiles[vector3.x, vector3.y].actorLayer = entity; }
            else if (id > 1000 && id <= 2000) { World.tiles[vector3.x, vector3.y].itemLayer = entity; }
            else { World.tiles[vector3.x, vector3.y].obstacleLayer = entity; }
            return entity;
        }
        public static Entity ReloadEntity(Entity entityRef)
        {
            if (entityRef != null)
            {
                Entity entityToUse = new Entity(entityRef);
                if (entityToUse.GetComponent<Inventory>() != null)
                {
                    List<Entity> entities = new List<Entity>();
                    if (entityToUse.GetComponent<Inventory>().inventory != null)
                    {
                        foreach (Entity entity in entityToUse.GetComponent<Inventory>().inventory) { if (entity != null) { entity.ClearImbeddedComponents(); entities.Add(entity); } }
                        entityToUse.GetComponent<Inventory>().inventory.Clear();
                        foreach (Entity id in entities) { entityToUse.GetComponent<Inventory>().inventory.Add(new Entity(id)); }

                        entities.Clear();
                        foreach (EquipmentSlot entity in entityToUse.GetComponent<Inventory>().equipment) { if (entity != null && entity.item != null) { entity.item.ClearImbeddedComponents(); entities.Add(entity.item); } }
                        foreach (Entity id in entities) { new Entity(id).GetComponent<Equippable>().Equip(entityToUse); }
                    }
                }
                if (CMath.ReturnAI(entityToUse) != null) { CMath.ReturnAI(entityToUse).SetTransitions(); }

                foreach (Component component in entityToUse.components)
                {
                    component.entity = entityToUse;
                }


                AddEntity(entityToUse);
                return entityToUse;
            }
            return null;
        }
    }
}
