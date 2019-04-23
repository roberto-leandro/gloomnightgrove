using UnityEngine;

public class PlayerCatMovementStrategy : AbstractPlayerMovementStrategy
{
    public PlayerCatMovementStrategy(PlayerController controller) : base(controller) { }



    public override Vector2 DetermineMovement()
    {
        Vector2 direction = new Vector2();

        // Resolve jumping
        if (playerController.Jump)
        {

            if (playerController.IsGrounded)
            {
                // Jump if the player is grounded
                direction.y = playerController.JumpForce;

            }
            else if (playerController.IsTouchingWallOnLeft || playerController.IsTouchingWallOnRight)
            {
                // If the player is touching a wall they are allowed to wall jump

                // Add x movement away from the wall
                if (playerController.IsTouchingWallOnLeft)
                {
                    playerController.LastWalljumpDirection = true; // true is right
                    direction.x = playerController.WallJumpSidewaysForce;

                }
                else if (playerController.IsTouchingWallOnRight)
                {
                    playerController.LastWalljumpDirection = false; // false is left
                    direction.x = playerController.WallJumpSidewaysForce * -1;
                }

                // Jump
                direction.y = playerController.WallJumpUpwardsForce;

                // Refund double jump for performing a double jump
                playerController.IsDoublejumpAvailable = true;

                playerController.LastWalljumpCounter = playerController.HinderedMovementAfterWalljumpDuration;

                //Debug.Log("Player is jumping while not grounded and touching a wall");
            }

            // The jump was resolved, set to false
            playerController.Jump = false;
        }

        //Debug.Log("fixedUpdatesSinceLastWallJump " + lastWalljumpCounter);

        // Resolve horizontal movement
        direction += base.DetermineHorizontalMovement();

        //Debug.Log("x velocity is "+direction.x);
        return direction;
    }
}