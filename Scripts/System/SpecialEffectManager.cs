using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace The_Ruins_of_Ipsus
{
    public class SpecialEffectManager
    {
        public static void Explosion(Entity originator, Vector2 origin, int strength)
        {
            Draw frame1 = new("Orange", "Clear", '+');
            Draw frame2 = new("Red", "Clear", 'x');
            Draw frame3 = new("Red_Orange", "Clear", (char)176);
            Draw[] frames = new Draw[3] { frame1, frame2, frame3 };
            List<Vector2> coordinates = RangeModels.SphereRangeModel(origin, strength, true);

            //List<Entity> particles = new List<Entity>();

            foreach (Vector2 coordinate in coordinates)
            {
                Entity particle = new Entity(new List<Component>
                        {
                            new Vector2(coordinate.x, coordinate.y),
                            frame1,
                            new ParticleComponent(World.random.Next(11, 14), World.random.Next(3, 5), "None", 2, frames)
                        });
                Renderer.AddParticle(coordinate.x, coordinate.y, particle);
                //particles.Add(particle);
            }

            //Renderer.StartAnimation(particles);

            foreach (Vector2 coordinate in coordinates)
            {
                if (World.tiles[coordinate.x, coordinate.y].obstacleLayer != null)
                {
                    World.tiles[coordinate.x, coordinate.y].obstacleLayer = null;
                }
                if (World.tiles[coordinate.x, coordinate.y].actorLayer != null)
                {
                    AttackManager.Attack(originator, World.tiles[coordinate.x, coordinate.y].actorLayer, new AttackFunction(strength, strength, strength, strength, "Fire"), "Explosion");
                }
            }
        }
        public static void SummonActor(Entity user, Vector2 targetLocation, int[] actors, int amountToSummon)
        {
            for (int i = 0; i < amountToSummon; i++)
            {
                foreach (int id in actors)
                {
                    Vector2 target = CMath.ReturnNearestValidCoordinate("Actor", targetLocation);
                    EntityManager.CreateEntity(target, id, false);

                    Draw frame1 = new Draw("Blue", "Black", '(');
                    Draw frame2 = new Draw("Blue", "Black", '-');
                    Draw frame3 = new Draw("Blue", "Black", ')');
                    Draw[] frames = new Draw[3] { frame1, frame2, frame3 };

                    List<Entity> particles = new List<Entity>();
                    List<Vector2> coordinates = RangeModels.SphereRangeModel(target, 10, true);
                    foreach (Vector2 coordinate in coordinates)
                    {
                        Entity particle = new Entity(new List<Component>
                        {
                            new Vector2(coordinate.x, coordinate.y),
                            frame1,
                            new ParticleComponent(World.random.Next(8, 10), 5, "Wander", 5, frames)
                        });
                        particles.Add(particle);
                    }
                    Renderer.StartAnimation(particles);
                }
            }
        }
        public static void BreathWeapon(Entity originator, Vector2 target, int strength, int range, string type)
        {
            List<Entity> particles = new List<Entity>();
            switch (type)
            {
                case "Fire":
                    {
                        Draw frame1 = new Draw("Red", "Black", 'x');
                        Draw frame2 = new Draw("Orange", "Black", '+');
                        Draw frame3 = new Draw("Red", "Black", (char)176);
                        Draw[] frames = new Draw[3] { frame1, frame2, frame3 };
                        List<Vector2> coordinates = RangeModels.ConeRangeModel(originator.GetComponent<Vector2>(), target, strength, range);
                        foreach (Vector2 coordinate in coordinates)
                        {
                            Entity particle = new Entity(new List<Component>
                                {
                                    new Vector2(coordinate.x, coordinate.y),
                                    frame1,
                                    new ParticleComponent(World.random.Next(22, 26), 5, "None", 1, frames)
                                });
                            particles.Add(particle);
                        }

                        Renderer.StartAnimation(particles);

                        foreach (var coordinate in coordinates)
                        {
                            if (World.tiles[coordinate.x, coordinate.y].obstacleLayer != null)
                            {
                                World.tiles[coordinate.x, coordinate.y].obstacleLayer = null;
                            }
                            if (World.tiles[coordinate.x, coordinate.y].actorLayer != null)
                            {
                                AttackManager.Attack(originator, World.tiles[coordinate.x, coordinate.y].actorLayer, new AttackFunction(strength, strength, strength, strength, "Fire"), "Fire");
                            }
                        }
                        break;
                    }
            }
        }
        public static void Lightning(Entity originator, Vector2 target, int strength, int range)
        {
            Draw frame1 = new Draw("Yellow", "Black", 'x');
            Draw frame2 = new Draw("Yellow", "Black", '+');
            Draw frame3 = new Draw("Yellow", "Black", (char)176);
            Draw[] frames = new Draw[3] { frame1, frame2, frame3 };

            List<Entity> particles = new List<Entity>();
            List<Vector2> coordinates = RangeModels.BeamRangeModel(originator.GetComponent<Vector2>(), target, range, false);
            foreach (Vector2 coordinate in coordinates)
            {
                Entity particle = new Entity(new List<Component>
                        {
                            new Vector2(coordinate.x, coordinate.y),
                            frame1,
                            new ParticleComponent(World.random.Next(22, 26), 5, "None", 1, frames)
                        });
                particles.Add(particle);

                if (World.random.Next(0, 2) == 1)
                {
                    Entity particle2 = new Entity(new List<Component>
                        {
                            new Vector2(coordinate.x + World.random.Next(-1, 2), coordinate.y + World.random.Next(-1, 2)),
                            frame1,
                            new ParticleComponent(World.random.Next(22, 26), 5, "None", 1, frames)
                        });
                    particles.Add(particle2);
                }
            }

            Renderer.StartAnimation(particles);

            foreach (Vector2 coordinate in coordinates)
            {
                Vector2 vector3 = coordinate;

                if (World.tiles[coordinate.x, coordinate.y].obstacleLayer != null)
                {
                    World.tiles[coordinate.x, coordinate.y].obstacleLayer = null;
                }
                if (World.tiles[coordinate.x, coordinate.y].actorLayer != null)
                {
                    AttackManager.Attack(originator, World.tiles[vector3.x, vector3.y].actorLayer,
                    new AttackFunction(strength, 8, 2, strength, "Lightning"), "Lightning");
                }
            }
        }
        public static void TongueLash(Entity originator, Vector2 target, int strength, int range)
        {
            List<Entity> particles = new List<Entity>();
            List<Vector2> coordinates = RangeModels.BeamRangeModel(originator.GetComponent<Vector2>(), target, range, false);
            foreach (Vector2 coordinate in coordinates)
            {
                if (coordinate == target)
                {
                    Entity particle = new Entity(new List<Component>
                        {
                            new Vector2(coordinate.x, coordinate.y),
                            new Draw("Pink", "Black", 'X'),
                            new ParticleComponent(4, 1, "None", 1, new Draw[] { new Draw("Pink", "Black", 'X')})
                        });
                    particles.Add(particle);
                }
                else
                {
                    Entity particle = new Entity(new List<Component>
                    {
                        new Vector2(coordinate.x, coordinate.y),
                        new Draw("Pink", "Black", '*'),
                        new ParticleComponent(4, 1, "None", 1, new Draw[] { new Draw("Pink", "Black", '*')})
                    });
                    particles.Add(particle);
                }
            }

            Renderer.StartAnimation(particles);

            if (World.tiles[target.x, target.y].actorLayer != null)
            {
                AttackManager.Attack(originator, World.tiles[target.x, target.y].actorLayer,
                new AttackFunction(strength, 8, 2, strength, "Bludgeoning"), "Tongue");
            }
        }
        public static void MagicMap(Entity entity)
        {
            Vector2 origin = entity.GetComponent<Vector2>();
            Draw frame1 = new Draw("Light_Yellow", "Light_Yellow", (char)0);
            Draw frame2 = new Draw("Yellow", "Yellow", (char)0);
            Draw[] frames = new Draw[2] { frame1, frame2 };

            List<Entity> particles = new List<Entity>();
            foreach (Traversable tile in World.tiles)
            {
                if (tile.terrainType != 0)
                {
                    Vector2 coordinate = tile.entity.GetComponent<Vector2>();
                    tile.entity.GetComponent<Visibility>().explored = true;
                    Entity particle = new Entity(new List<Component>
                        {
                            new Vector2(coordinate.x, coordinate.y),
                            frame1,
                            new ParticleComponent((int)CMath.Distance(origin.x, origin.y, coordinate.x, coordinate.y), 4, "None", 0, frames)
                        });
                    particles.Add(particle);
                }
            }

            Renderer.StartAnimation(particles);
        }
    }
}
