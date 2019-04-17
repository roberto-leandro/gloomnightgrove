using UnityEngine;

public class PlayerCrowMovementStrategy : AbstractPlayerMovementStrategy
{

    public PlayerCrowMovementStrategy(PlayerController controller) : base(controller) { }

    public override Vector2 DetermineMovement()
    {

        Vector2 direction = new Vector2();

        // Handle jump
        if (jump)
        {
            // In crow mode, player can always jump if they are grounded
            // They can also jump if airborne and they have a doublejump available
            if (playerController.IsGrounded || playerController.IsDoublejumpAvailable)
            {
                if (!playerController.IsGrounded)
                {
                    // Remove the player's double jump
                    playerController.IsDoublejumpAvailable = false;
                }

                // Add the jump direction
                direction.y += playerController.JumpForce;

                // Player is not grounded anymore
                playerController.IsGrounded = false;
            }
            // The jump was resolved, set to false
            jump = false;
        }

        // Handle horizontal movement
        direction.x = horizontalMovement;
        Debug.Log(direction);
        return direction;
    }


}
