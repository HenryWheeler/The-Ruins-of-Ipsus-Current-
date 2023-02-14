using SadConsole.Entities;
using System.Collections.Generic;
using The_Ruins_of_Ipsus.Scripts.JsonDataManagement;

namespace The_Ruins_of_Ipsus
{
    public class EntityManager
    {
        public static Dictionary<string, List<Entity>> actors = new Dictionary<string, List<Entity>>();
        public static List<Entity> items = new List<Entity>();
        public static List<Entity> obstacles = new List<Entity>();
        public static int currentEntity = 1;
        public static int actorCount = 0;
        public static int itemCount = 0;
        public static int obstacleCount = 0;
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
        public static void ClearAllIndexes()
        {
            foreach (List<Entity> entities in actors.Values)
            {
                foreach (Entity actor in entities)
                {
                    if (actor != null && actor.GetComponent<TurnFunction>() != null && actor.GetComponent<PlayerComponent>() == null)
                    { 
                        TurnManager.RemoveActor(actor.GetComponent<TurnFunction>()); 
                    }
                }
            }
            actorCount = 0;
            itemCount = 0;
            obstacleCount = 0;
            actors.Clear();
            items.Clear();
            obstacles.Clear();
        }
        public static void AddEntity(Entity entity, bool update = true)
        {
            if (entity != null)
            {
                string type = entity.GetComponent<ID>().entityType;
                if (type == "Actor")
                {
                    string faction = entity.GetComponent<Faction>().faction;
                    if (!actors.ContainsKey(faction))
                    {
                        actors.Add(faction, new List<Entity>());
                    }
                    actors[faction].Add(entity);

                    actorCount++;
                }
                else if (type == "Item")
                {
                    items.Add(entity);

                    itemCount++;
                }
                else if (type == "Obstacle")
                {
                    obstacles.Add(entity);

                    obstacleCount++;
                }

                if (update)
                {
                    UpdateMap(entity);
                }
            }
        }
        public static void RemoveEntity(Entity entity, bool update = true)
        {
            string type = entity.GetComponent<ID>().entityType;
            if (type == "Actor")
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

                actorCount--;
            }
            else if (type == "Item")
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

                itemCount--;
            }
            else if(type == "Obstacle")
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

                obstacleCount--;
            }
        }
        public static void UpdateMap(Entity entity)
        {
            if (entity != null)
            {
                string type = entity.GetComponent<ID>().entityType;
                if (type == "Actor")
                {
                    string reference = entity.GetComponent<ID>().ReturnInstanceReference();
                    DijkstraMaps.CreateMap(entity, reference, 25);
                    DijkstraMaps.CreateReverseMap(reference, $"{reference}-Fear");
                }
                else if (type == "Item")
                {
                    DijkstraMaps.CreateMap(entity, entity.GetComponent<ID>().ReturnInstanceReference(), 25);
                    DijkstraMaps.CreateMap(items, "Items", 100);
                }
                else if (type == "Obstacle")
                {
                    DijkstraMaps.CreateMap(entity, entity.GetComponent<ID>().ReturnInstanceReference(), 25);
                    DijkstraMaps.CreateMap(obstacles, "Obstacles", 100);
                }
            }
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
        public static Entity CreateEntityFromFile(Vector2 position, string fileName, bool place = true)
        {
            Entity entity = JsonDataManager.ReturnEntity(fileName);

            if (CMath.ReturnAI(entity) != null)
            {
                CMath.ReturnAI(entity).SetTransitions();

                TurnManager.AddActor(entity.GetComponent<TurnFunction>());

                if (entity.GetComponent<Faction>().faction == "Evil Humanoid")
                {
                    SpawnTableManager.GiveRandomItems(entity, true);
                    entity.RemoveComponent(entity.GetComponent<PronounSet>());
                    if (World.seed.Next(1, 101) > 50) { entity.AddComponent(PronounReferences.pronounSets["Masculine"]); }
                    else { entity.AddComponent(PronounReferences.pronounSets["Feminine"]); }
                }
            }

            AddEntity(entity);

            entity.GetComponent<Vector2>().x = position.x;
            entity.GetComponent<Vector2>().y = position.y;

            entity.GetComponent<ID>().temporaryID = currentEntity;
            currentEntity++;

            if (place)
            {
                string type = entity.GetComponent<ID>().entityType;
                if (type == "Actor") { World.tiles[position.x, position.y].actorLayer = entity; }
                else if (type == "Item") { World.tiles[position.x, position.y].itemLayer = entity; }
                else if (type == "Obstacle") { World.tiles[position.x, position.y].obstacleLayer = entity; }
            }

            if (entity.components == null) { Log.Add("THINGS ARE BROKEN"); }

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

            entity.GetComponent<ID>().temporaryID = currentEntity;
            currentEntity++;

            string type = entity.GetComponent<ID>().entityType;
            if (type == "Actor") { World.tiles[vector3.x, vector3.y].actorLayer = entity; }
            else if (type == "Item") { World.tiles[vector3.x, vector3.y].itemLayer = entity; }
            else if (type == "Obstacle") { World.tiles[vector3.x, vector3.y].obstacleLayer = entity; }

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

                entityToUse.GetComponent<ID>().temporaryID = currentEntity;
                currentEntity++;

                AddEntity(entityToUse);
                return entityToUse;
            }
            return null;
        }
    }
}
