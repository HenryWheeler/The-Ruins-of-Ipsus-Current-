using System;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    class UpdateCameraOnMove : OnMove
    {
        public override void Move(Vector2 initialPosition, Vector2 finalPosition) { Renderer.MoveCamera(entity.GetComponent<Vector2>()); }
        public UpdateCameraOnMove() { }
    }
}
