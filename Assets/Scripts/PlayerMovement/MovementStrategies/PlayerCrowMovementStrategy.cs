using UnityEngine;

/// <summary>
/// Defines the player's movement when the current animal is the crow, including the double jump mechanic.
/// </summary>
public class PlayerCrowMovementStrategy : AbstractPlayerMovementStrategy
{
    public PlayerCrowMovementStrategy(PlayerController controller) : base(controller) { }

    /// <summary>
    /// Determine the movement for the player while in crow mode. In this mode the player has access to one double jump.
    /// </summary>
    /// <returns></returns>
    public override Vector2 DetermineMovement()
    {
        Vector2 direction = new Vector2();

        // Handle jump
        if (characterController.Jump)
        {
            // In crow mode, player can always jump if they are grounded
            // They can also jump if airborne and they have a doublejump available
            if (characterController.IsGrounded || characterController.IsDoublejumpAvailable)
            {
                if (!characterController.IsGrounded)
                {
                    // Remove the player's double jump
                    characterController.IsDoublejumpAvailable = false;
                }

                // Add the jump direction
                direction.y += characterController.JumpForce;   
            }

            // The jump was resolved, set to false
            characterController.Jump = false;
        }

        // Handle horizontal movement
        direction += base.DetermineHorizontalMovement();

        return direction;
    }


}
