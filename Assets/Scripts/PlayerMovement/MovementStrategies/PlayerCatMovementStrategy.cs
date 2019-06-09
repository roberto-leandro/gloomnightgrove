using UnityEngine;

/// <summary>
/// Defines the player's movement when the current animal is the cat, including the wall jump mechanic.
/// </summary>
public class PlayerCatMovementStrategy : AbstractPlayerMovementStrategy
{
    public PlayerCatMovementStrategy(PlayerController controller) : base(controller) { }

    protected override Vector2 DetermineVerticalMovement()
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
                    characterController.WallHitstunDirection = true; // true is right
                    direction.x = characterController.WallJumpSidewaysForce;
                }
                else if (characterController.IsTouchingWallOnRight)
                {
                    characterController.WallHitstunDirection = false; // false is left
                    direction.x = characterController.WallJumpSidewaysForce * -1;
                }

                // Jump
                direction.y = characterController.WallJumpUpwardsForce;

                // Refund double jump for performing a double jump
                characterController.IsDoublejumpAvailable = true;

                // Set this variable so the game knows that player wall jumped X frames ago and cotinues giving them a momentum away from the wall
                characterController.WallHitstunCounter = characterController.WalljumpMovementDuration;

                //Debug.Log("Player is jumping while not grounded and touching a wall");
            }

            // The jump was resolved, set to false
            characterController.Jump = false;
        }

        //Debug.Log("x velocity is "+direction.x);
        return direction;
    }
}