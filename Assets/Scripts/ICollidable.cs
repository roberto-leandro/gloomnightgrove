using UnityEngine;

interface ICollidable
{
    RaycastHit2D[] FindCollisions(Vector2 direction);
    void HandleCollision(RaycastHit2D raycast);
}
