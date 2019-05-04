using UnityEngine;

/// <summary>
/// Defines the player's movement when the current animal is the cat, including the wall jump mechanic.
/// </summary>
public class PlayerCatMovementStrategy : AbstractPlayerMovementStrategy
{
    public PlayerCatMovementStrategy(PlayerController controller) : base(controller) { }

    public override Vector2 DetermineMovement()
    {
        Vector2 direction = new Vector2();

        // Resolve jumping
        if (characterController.Jump)
        {

            if (characterController.IsGrounded)
            {
                // Jump if the player is grounded
                direction.y = characterController.JumpForce;

            }
            else if (characterController.IsTouchingWallOnLeft || characterController.IsTouchingWallOnRight)
            {
                // If the player is touching a wall they are allowed to wall jump

                // Add x movement away from the wall
                if (characterController.IsTouchingWallOnLeft)
                {
                    characterController.LastWalljumpDirection = true; // true is right
                    direction.x = characterController.WallJumpSidewaysForce;

                }
                else if (characterController.IsTouchingWallOnRight)
                {
                    characterController.LastWalljumpDirection = false; // false is left
                    direction.x = characterController.WallJumpSidewaysForce * -1;
                }

                // Jump
                direction.y = characterController.WallJumpUpwardsForce;

                // Refund double jump for performing a double jump
                characterController.IsDoublejumpAvailable = true;

                characterController.LastWalljumpCounter = characterController.HinderedMovementAfterWalljumpDuration;

                //Debug.Log("Player is jumping while not grounded and touching a wall");
            }

            // The jump was resolved, set to false
            characterController.Jump = false;
        }

        //Debug.Log("fixedUpdatesSinceLastWallJump " + lastWalljumpCounter);

        // Resolve horizontal movement
        direction += base.DetermineHorizontalMovement();

        //Debug.Log("x velocity is "+direction.x);
        return direction;
    }
}