using System;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class Mimicry : Component
    {
        public bool disguised = false;
        public bool CaptureGuise()
        {
            Vector2 vector2 = entity.GetComponent<Vector2>();
            for (int x = vector2.x - 1; x < vector2.x + 1; x++)
            {
                for (int y = vector2.y - 1; y < vector2.y + 1; y++)
                {
                    Traversable traversable = World.tiles[x, y];
                    if (traversable.actorLayer != null && traversable.actorLayer != entity)
                    {
                        Disguise(traversable.actorLayer.GetComponent<Draw>());
                        disguised = true;
                        entity.GetComponent<TurnFunction>().EndTurn();
                        return true;
                    }
                    else if (traversable.itemLayer != null)
                    {
                        Disguise(traversable.itemLayer.GetComponent<Draw>());
                        disguised = true;
                        entity.GetComponent<TurnFunction>().EndTurn();
                        return true;
                    }
                    else if (traversable.obstacleLayer != null)
                    {
                        Disguise(traversable.obstacleLayer.GetComponent<Draw>());
                        disguised = true;
                        entity.GetComponent<TurnFunction>().EndTurn();
                        return true;
                    }
                }
            }
            disguised = false;
            return false;
        }
        public void Disguise(Draw newDraw)
        {
            Draw draw = entity.GetComponent<Draw>();

            draw.character = newDraw.character;
            draw.fColor = newDraw.fColor;
            draw.bColor = newDraw.bColor;

            Log.Add("Mimic assumes a new form!");
        }
        public Mimicry() { }
    }
}
