using UnityEngine;

/// <summary>
/// Implements a movement strategy that constantly moves a character between two points or walls.
/// </summary>
public class BackAndForthMovementStrategy : AbstractMovementStrategy<EnemyController>
{
    public BackAndForthMovementStrategy(EnemyController controller) : base(controller) { }

    public override Vector2 DetermineMovement()
    {
        // Assume character is traveling to the right
        Vector2 direction = new Vector2(characterController.MovementSpeed, 0);
        
        // Character only moves to the left in two scenarios:
        // 1. A wall was touched on the right
        // No wall is being touched and the character is already facing left
        if (characterController.IsTouchingWallOnRight || (!characterController.IsTouchingWallOnLeft && !characterController.IsFacingRight))
        {
            direction.x = -1 * characterController.MovementSpeed;
        }

        return direction;
    }
}
