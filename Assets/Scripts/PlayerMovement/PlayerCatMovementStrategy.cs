using UnityEngine;

public class PlayerCatMovementStrategy : AbstractPlayerMovementStrategy
{
    public PlayerCatMovementStrategy(PlayerController controller) : base(controller) {}

    public override Vector2 DetermineMovement()
    {
        // Handle horizontal movement
        throw new System.NotImplementedException();
    }
}