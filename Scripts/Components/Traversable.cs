namespace The_Ruins_of_Ipsus
{
    public class Traversable : Component
    {
        public int terrainType { get; set; }
        public Entity actorLayer { get; set; }
        public Entity itemLayer { get; set; }
        public Entity obstacleLayer { get; set; }
        public Traversable(int _terrainType) { terrainType = _terrainType; }
        public Traversable() { }
    }
}
