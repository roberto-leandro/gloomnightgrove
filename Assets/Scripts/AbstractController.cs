using UnityEngine;

public abstract class AbstractController : MonoBehaviour, ICollidable, IMovable
{
    public IMovementStrategy movementStrategy;
    public Rigidbody2D rigidBody;
    public Vector2 velocity;

    public RaycastHit2D[] FindCollisions(Vector2 direction)
    {
        throw new System.NotImplementedException();
    }

    public void HandleCollision(RaycastHit2D raycast)
    {
        throw new System.NotImplementedException();
    }

    public void Move(Vector2 direction)
    {
        throw new System.NotImplementedException();
    }
}
