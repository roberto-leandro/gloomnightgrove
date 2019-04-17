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
            if (playerController.IsGrounded) {
                Debug.Log("player normal jumped");
                direction.y += playerController.JumpForce;
                playerController.IsGrounded = false;
            } else if (playerController.IsDoublejumpAvailable)
            {
                Debug.Log("player double jumped");
                direction.y += playerController.JumpForce;
                playerController.IsGrounded = false;
                playerController.IsDoublejumpAvailable = false;
            }
        }

            /*if (jump)
            {
                // In crow mode, player can always jump if they are grounded
                // They can also jump if airborne and they have a doublejump available
                if (playerController.IsGrounded || playerController.IsDoublejumpAvailable)
                {
                    // Add the jump direction
                    direction.y += playerController.JumpForce;

                    if (!playerController.IsGrounded)
                    {
                        // Remove the player's double jump
                        playerController.IsDoublejumpAvailable = false;
                    }

                    // Player is not grounded anymore
                    playerController.IsGrounded = false; 
                }
            }*/

            // Handle horizontal movement
            direction.x = horizontalMovement;

        //Debug.Log("the plyaer will move to" + direction.ToString());
        return direction;
    }


}
