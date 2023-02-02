using SadConsole;
using System.Collections.Generic;
using SadRogue.Primitives;
using SadConsole.Input;
using System;
using Console = SadConsole.Console;
using SadConsole.Entities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using SadConsole.Readers;

namespace The_Ruins_of_Ipsus
{
    public class Program
    {
        private static readonly int screenWidth = 100;
        private static readonly int screenHeight = 50;
        public static RootConsole rootConsole;

        // The map console takes up most of the screen and is where the map will be drawn.
        private static readonly int mapWidth = 65;
        private static readonly int mapHeight = 50;
        public static TitleConsole mapConsole;
        // Below the map console is the message console which outputs messages to the player.
        private static readonly int messageWidth = 35;
        private static readonly int messageHeight = 15;
        public static TitleConsole logConsole;
        // The player console or "rogue" console displays player information, such as stats and status effects.
        private static readonly int rogueWidth = 35;
        private static readonly int rogueHeight = 35;
        public static TitleConsole playerConsole;
        // The inventory console displays the player's inventory and will allow for navigation through the inventory.
        private static readonly int inventoryWidth = 35;
        private static readonly int inventoryHeight = 50;
        public static TitleConsole inventoryConsole;
        // The equipment console displays the player's equipment and will allow for equipment display.
        private static readonly int equipmentWidth = 35;
        private static readonly int equipmentHeight = 50;
        public static TitleConsole equipmentConsole;
        // The target console will display information about targeting.
        private static readonly int targetWidth = 35;
        private static readonly int targetHeight = 50;
        public static TitleConsole targetConsole;
        // The target console will display information about looking.
        private static readonly int lookWidth = 35;
        private static readonly int lookHeight = 50;
        public static TitleConsole lookConsole;

        public static Entity player;
        public static bool gameActive = false;

        public static int gameMapWidth = 100;
        public static int gameMapHeight = 100;
        private static void Main(string[] args)
        {
            Settings.WindowTitle = "The Ruins of Ipsus";

            Game.Create(screenWidth, screenHeight, "fonts/ascii_6x6.font.json");
            Game.Instance.DefaultFontSize = IFont.Sizes.Two;
            Game.Instance.OnStart = Init;
            Game.Instance.Run();
            Game.Instance.Dispose();
        }
        private static void Init()
        {
            rootConsole = new RootConsole(Game.Instance.ScreenCellsX, Game.Instance.ScreenCellsY);

            mapConsole = new TitleConsole("< Map >", mapWidth, mapHeight) { Position = new Point(0, 0) };
            logConsole = new TitleConsole("< Message Log >", messageWidth, messageHeight) { Position = new Point(mapWidth, rogueHeight) };
            playerConsole = new TitleConsole("< The Rogue @ >", rogueWidth, rogueHeight) { Position = new Point(mapWidth, 0) };
            inventoryConsole = new TitleConsole("< Inventory >", inventoryWidth, inventoryHeight) { Position = new Point(mapWidth, 0) };
            equipmentConsole = new TitleConsole("< Equipment >", equipmentWidth, equipmentHeight) { Position = new Point(mapWidth, 0) };
            targetConsole = new TitleConsole("< Targeting >", targetWidth, targetHeight) { Position = new Point(mapWidth, 0) };
            lookConsole = new TitleConsole("< Looking >", lookWidth, lookHeight) { Position = new Point(mapWidth, 0) };

            rootConsole.Children.Add(mapConsole);
            rootConsole.Children.Add(logConsole);
            rootConsole.Children.Add(playerConsole);
            rootConsole.Children.Add(inventoryConsole);
            rootConsole.Children.Add(targetConsole);
            rootConsole.Children.Add(lookConsole);

            Game.Instance.Keyboard.InitialRepeatDelay = .4f;
            Game.Instance.Keyboard.RepeatDelay = .05f;
            rootConsole.SadComponents.Add(new KeyboardComponent());

            Game.Instance.Screen = rootConsole;
            Game.Instance.Screen.IsFocused = true;
            rootConsole.Children.MoveToTop(logConsole);
            rootConsole.Children.MoveToTop(playerConsole);

            // This is needed because we replaced the initial screen object with our own.
            Game.Instance.DestroyDefaultStartingConsole();

            LoadFunctions();
        }
        public static void ExitProgram()
        {
            gameActive = false;
            Renderer.running = false; 
            System.Environment.Exit(1);
        }
        public static void LoadFunctions()
        {
            Renderer renderer = new Renderer(mapConsole, mapWidth, mapHeight);
            Log log = new Log(logConsole);
            StatManager stats = new StatManager(playerConsole);
            SaveDataManager saveDataManager = new SaveDataManager();
            JsonDataManager jsonDataManager = new JsonDataManager();
            PronounReferences pronounReferences = new PronounReferences();
            //SpawnTableManager spawnTableManager = new SpawnTableManager();
            DijkstraMaps dijkstraMaps = new DijkstraMaps(gameMapWidth, gameMapHeight);
            EntityManager.LoadAllEntities();

            //SaveDataManager.LoadSave();
            NewGame();
        }
        public static void LoadPlayerFunctions(Entity player)
        {
            InventoryManager inventory = new InventoryManager(logConsole, player);
            Look look = new Look(player);
            TargetReticle reticle = new TargetReticle(player);
        }
        public static void ReloadPlayer(List<Component> components)
        {
            player = EntityManager.ReloadEntity(new Entity(components));
            //Thread thread = new Thread(() => player.GetComponent<PlayerComponent>().Update());
            //thread.Start();
            //rootConsole.Update += player.GetComponent<PlayerComponent>().Update;

            Vector2 vector2 = player.GetComponent<Vector2>();
            World.tiles[vector2.x, vector2.y].actorLayer = player;
            StatManager.UpdateStats(player);
            TurnManager.AddActor(player.GetComponent<TurnFunction>());
            //Action.PlayerAction(player);
            ShadowcastFOV.Compute(vector2, player.GetComponent<Stats>().sight);
            player.GetComponent<UpdateCameraOnMove>().Move(vector2, vector2);
            player.GetComponent<TurnFunction>().StartTurn();

            EntityManager.AddEntity(player);
        }
        public static void CreateNewPlayer()
        {
            player = new Entity();
            player.AddComponent(new ID(0));
            player.AddComponent(new Vector2(0, 0));
            player.AddComponent(new Draw("White", "Black", '@'));
            player.AddComponent(new Description("You", "It's you."));
            player.AddComponent(PronounReferences.pronounSets["Player"]);
            player.AddComponent(new Stats(10, 10, 1f, 50, 1, 1));
            player.AddComponent(new TurnFunction());
            player.AddComponent(new Movement(new List<int> { 1, 2 }));
            player.AddComponent(new Inventory());
            player.AddComponent(new Harmable());
            player.AddComponent(new Faction("Player"));
            player.AddComponent(new UpdateCameraOnMove());
            player.AddComponent(new PlayerComponent());
            Entity startingWeapon = new Entity(new List<Component>()
            {
                new Vector2(0, 0),
                new ID(1100),
                new Draw("Orange", "Black", '!'),
                new Description("Potion of Orange*Explosion", "The label reads: 'Do Not Drink'."),
                new Usable("The potion explodes in a Red*fiery burst!"),
                new Throwable("The potion explodes in a Red*fiery burst!"),
                new ExplodeOnUse(6, 0),
                new ExplodeOnThrow(6),
            });
            InventoryManager.AddToInventory(player, new Entity(startingWeapon));
            InventoryManager.AddToInventory(player, new Entity(startingWeapon));
            InventoryManager.AddToInventory(player, new Entity(startingWeapon));
            InventoryManager.AddToInventory(player, new Entity(startingWeapon));

            Entity testScrollOfLightning = new Entity(new List<Component>()
            {
                new Vector2(0, 0),
                new ID(1300),
                new Draw("Yellow", "Black", '?'),
                new Description("Scroll of Yellow*Lightning", "This scroll is carved with Yellow*yellow Yellow*runes onto a vellum of human skin."),
                new Usable("A bolt of Yellow*lightning crackles and fries the air in front of the scroll!", false),
                new LightningOnUse(5, 15),
            });
            InventoryManager.AddToInventory(player, new Entity(testScrollOfLightning));
            InventoryManager.AddToInventory(player, new Entity(testScrollOfLightning));
            InventoryManager.AddToInventory(player, new Entity(testScrollOfLightning));
            InventoryManager.AddToInventory(player, new Entity(testScrollOfLightning));
            InventoryManager.AddToInventory(player, new Entity(testScrollOfLightning));

            Entity testMagicMappingScroll = new Entity(new List<Component>()
            {
                new Vector2(0, 0),
                new ID(1301),
                new Draw("Cyan", "Black", '?'),
                new Description("Scroll of Cyan*Mapping", "This scroll seems as if lighter than air and feels charged with unearthly knowledge."),
                new Usable("The world around you becomes Cyan*clearer."),
                new MagicMapOnUse(),
            });
            InventoryManager.AddToInventory(player, new Entity(testMagicMappingScroll));
            InventoryManager.AddToInventory(player, new Entity(testMagicMappingScroll));
            InventoryManager.AddToInventory(player, new Entity(testMagicMappingScroll));
            InventoryManager.AddToInventory(player, new Entity(testMagicMappingScroll));

            Entity djinnInABottle = new Entity(new List<Component>()
            {
                new Vector2(0, 0),
                new ID(1101),
                new Draw("Cyan", "Black", '!'),
                new Description("Cyan*Djinn in a Bottle", "This glass bottle is filled with a furious Cyan*Djinn."),
                new Usable("The bottle cracks open and a furious Cyan*Djinn emerges!"),
                new Throwable("The bottle cracks open and a furious Cyan*Djinn emerges!"),
                new SummonActorOnUse(new int[] { 75 }, 1, 0),
                new SummonActorOnThrow(new int[] { 75 }, 1),
            });
            InventoryManager.AddToInventory(player, new Entity(djinnInABottle));
            InventoryManager.AddToInventory(player, new Entity(djinnInABottle));
            InventoryManager.AddToInventory(player, new Entity(djinnInABottle));
            InventoryManager.AddToInventory(player, new Entity(djinnInABottle));

            Entity testPotionOfDragonsFire = new Entity(new List<Component>()
            {
                new Vector2(0, 0),
                new ID(1102),
                new Draw("Red", "Black", '!'),
                new Description("Potion of Red*Dragon's Red*Fire", "The label reads: 'Breathe with the fire of Red*Dragons!'"),
                new Usable($"A cone of Red*flame emits from your mouth!", false),
                new BreathWeaponOnUse(5, "Fire", 10),
            });
            InventoryManager.AddToInventory(player, new Entity(testPotionOfDragonsFire));
            InventoryManager.AddToInventory(player, new Entity(testPotionOfDragonsFire));
            InventoryManager.AddToInventory(player, new Entity(testPotionOfDragonsFire));
            InventoryManager.AddToInventory(player, new Entity(testPotionOfDragonsFire));
            InventoryManager.AddToInventory(player, new Entity(testPotionOfDragonsFire));

            Entity testTongueLash = new Entity(new List<Component>()
            {
                new Vector2(0, 0),
                new ID(1932),
                new Draw("Pink", "Black", '/'),
                new Description("Pink*Tongue Test", "Test"),
                new Usable($"A giant Pink*Tongue emits from your mouth!", false),
                new TongueLashOnUse(5, 100),
            });
            InventoryManager.AddToInventory(player, new Entity(testTongueLash));
            InventoryManager.AddToInventory(player, new Entity(testTongueLash));
            InventoryManager.AddToInventory(player, new Entity(testTongueLash));
            InventoryManager.AddToInventory(player, new Entity(testTongueLash));
            InventoryManager.AddToInventory(player, new Entity(testTongueLash));

            //Action.PlayerAction(player);
            EntityManager.AddEntity(player);
            TurnManager.AddActor(player.GetComponent<TurnFunction>());
            StatManager.UpdateStats(player);
            player.GetComponent<TurnFunction>().StartTurn();
        }
        public static void NewGame()
        {
            gameActive = true;
            World world = new World(gameMapWidth, gameMapHeight);
            CreateNewPlayer();
            World.GenerateNewFloor(true);
            LoadPlayerFunctions(player);
            Log.Add("Welcome to the Ruins of Ipsus");
            Log.DisplayLog();

            Renderer.DrawMapToScreen();
            //EntityManager.CreateNewEntityTest();
        }
        public static void LoadSave(SaveData saveData)
        {
            EntityManager.LoadAllEntities();
            World world = new World(gameMapWidth, gameMapHeight, saveData.depth, saveData.seed);

            foreach (Entity actor in saveData.actors) { if (actor != null) { Entity entity = EntityManager.ReloadEntity(actor); World.tiles[entity.GetComponent<Vector2>().x, entity.GetComponent<Vector2>().y].actorLayer = entity; } }
            foreach (Entity item in saveData.items) { if (item != null) { Entity entity = EntityManager.ReloadEntity(item); World.tiles[entity.GetComponent<Vector2>().x, entity.GetComponent<Vector2>().y].itemLayer = entity; } }
            foreach (Entity terrain in saveData.terrain) { if (terrain != null) { Entity entity = EntityManager.ReloadEntity(terrain); World.tiles[entity.GetComponent<Vector2>().x, entity.GetComponent<Vector2>().y].obstacleLayer = entity; } }
            foreach (Traversable tile in World.tiles) { if (tile != null && tile.entity != null) { Vector2 vector2 = tile.entity.GetComponent<Vector2>(); if (saveData.visibility[vector2.x, vector2.y] != null) { tile.entity.RemoveComponent(tile.entity.GetComponent<Visibility>()); tile.entity.AddComponent(saveData.visibility[vector2.x, vector2.y]); } } }

            ReloadPlayer(saveData.player.components);
            LoadPlayerFunctions(player);
            RecordKeeper.record = saveData.records;
            Renderer.MoveCamera(player.GetComponent<Vector2>());
            Log.Add("Welcome to the Ruins of Ipsus");
            Log.DisplayLog();
            gameActive = true;
        }
    }
    public class RootConsole : Console
    {
        public List<ParticleComponent> particles = new List<ParticleComponent>();
        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            if (particles.Count > 0)
            {
                World.ClearSFX();

                for (int i = 0; i < particles.Count; i++)
                {
                    ParticleComponent particle = particles[i];
                    particle?.Progress();
                }

                if (particles.Count == 0)
                {
                    World.ClearSFX();
                }

                The_Ruins_of_Ipsus.Renderer.DrawMapToScreen();

                IsDirty = true;
            }
        }
        public RootConsole(int _width, int _height)
            : base(_width, _height) { }
    }
    public class TitleConsole : Console
    {
        public string title { get; set; }
        public TitleConsole(string _title, int _width, int _height)
            : base(_width, _height)
        {
            title = _title;
            this.Fill(Color.Black, Color.Black, 176);

            The_Ruins_of_Ipsus.Renderer.CreateConsoleBorder(this);
        }
    }
    public class ParticleComponent : Component
    {
        public int life { get; set; }
        public int speed { get; set; }
        public string direction { get; set; }
        public int threshold { get; set; }
        public int currentThreshold = 0;
        public Draw[] particles { get; set; }
        public int currentParticle = 0;
        public bool animation = false;
        public void Progress()
        {

            currentThreshold--;

            if (currentThreshold <= 0)
            {
                Vector2 position = entity.GetComponent<Vector2>();

                currentThreshold = threshold;

                //The kind of movement a particle will display.
                switch (direction)
                {
                    case "Attached":
                        {
                            //Work for later, go make it so the particle that can be stuck to an entity.
                            break;
                        }
                    case "Target":
                        {
                            Vector2 newPosition = DijkstraMaps.PathFromMap(position, "ParticlePath");
                            entity.GetComponent<Vector2>().x = newPosition.x;
                            entity.GetComponent<Vector2>().y = newPosition.y;
                            break;
                        }
                    case "Wander":
                        {
                            position.x += World.random.Next(-1, 2);
                            position.y += World.random.Next(-1, 2);
                            break;
                        }
                    case "None": { break; }
                    case "North":
                        {
                            position.y--;
                            break;
                        }
                    case "NorthEast":
                        {
                            position.x--;
                            position.y--;
                            break;
                        }
                    case "East":
                        {
                            position.x--;
                            break;
                        }
                    case "SouthEast":
                        {
                            position.x--;
                            position.y++;
                            break;
                        }
                    case "South":
                        {
                            position.y++;
                            break;
                        }
                    case "SouthWest":
                        {
                            position.x++;
                            position.y++;
                            break;
                        }
                    case "West":
                        {
                            position.x++;
                            break;
                        }
                    case "NorthWest":
                        {
                            position.x++;
                            position.y--;
                            break;
                        }
                    case "WanderNorth":
                        {
                            position.x += World.random.Next(-1, 2);
                            position.y += World.random.Next(-1, 0);
                            break;
                        }
                    case "WanderNorthEast":
                        {
                            position.x += World.random.Next(-1, 0);
                            position.y += World.random.Next(-1, 0);
                            break;
                        }
                    case "WanderEast":
                        {
                            position.x += World.random.Next(-1, 0);
                            position.y += World.random.Next(-1, 2);
                            break;
                        }
                    case "WanderSouthEast":
                        {
                            position.x += World.random.Next(-1, 0);
                            position.y += World.random.Next(0, 2);
                            break;
                        }
                    case "WanderSouth":
                        {
                            position.x += World.random.Next(-1, 2);
                            position.y += World.random.Next(0, 2);
                            break;
                        }
                    case "WanderSouthWest":
                        {
                            position.x += World.random.Next(0, 2);
                            position.y += World.random.Next(0, 2);
                            break;
                        }
                    case "WanderWest":
                        {
                            position.x += World.random.Next(0, 2);
                            position.y += World.random.Next(-1, 2);
                            break;
                        }
                    case "WanderNorthWest":
                        {
                            position.x += World.random.Next(0, 2);
                            position.y += World.random.Next(-1, 0);
                            break;
                        }
                }

                //If in bounds display the particle to the map.
                if (CMath.CheckBounds(position.x, position.y))
                {
                    World.sfx[position.x, position.y] = entity.GetComponent<Draw>();
                }

                if (currentParticle != particles.Length - 1)
                {
                    currentParticle++;
                }
                else
                {
                    currentParticle = 0;
                }

                Draw draw = entity.GetComponent<Draw>();
                draw.character = particles[currentParticle].character;
                draw.fColor = particles[currentParticle].fColor;
                draw.bColor = particles[currentParticle].bColor;

                life--;
                if (life <= 0)
                {
                    KillParticle();
                    return;
                }
            }
        }
        public void KillParticle()
        {
            Program.rootConsole.particles.Remove(this);
        }
        public ParticleComponent(int _life, int _speed, string _direction, int _threshHold, Draw[] _particles, Vector2 target = null, bool _animation = false)
        {
            life = _life;
            speed = _speed;
            direction = _direction;
            threshold = _threshHold;
            particles = _particles;

            if (target != null)
            {
                DijkstraMaps.CreateMap(target, "ParticlePath");
            }

            if (_animation)
            {
                animation = true;
            }
        }
    }
}