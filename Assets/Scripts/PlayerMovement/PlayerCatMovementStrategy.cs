using UnityEngine;

public class PlayerCatMovementStrategy : AbstractPlayerMovementStrategy
{
    public PlayerCatMovementStrategy(PlayerController controller) : base(controller) { }

    public override Vector2 DetermineMovement()
    {
        // Handle horizontal movement
        Vector2 direction = new Vector2();

        

        if (jump)
        {
            
            if (playerController.IsGrounded)
            {
                // If the player is grounded jump
                direction.y = playerController.JumpForce;
                
            }
            else if(playerController.IsTouchingWallOnLeft || playerController.IsTouchingWallOnRight)
            {
                // If the player is touching a wall

                // X movement awat from the wall
                if (playerController.IsTouchingWallOnLeft)
                {
                    
                    if (direction.x < 0)
                    {
                        direction.x = 20;
                    }

                }
                else if (playerController.IsTouchingWallOnRight)
                {
                    
                    if (direction.x > 0)
                    {
                        direction.x = -20;
                    }
                }

                // Jump
                direction.y = playerController.JumpForce;

                // Set wall touching to false and unlock the y axis
                playerController.IsTouchingWallOnLeft = false;
                playerController.IsTouchingWallOnRight = false;
                playerController.Rigidbody.constraints = RigidbodyConstraints2D.None;
                
                Debug.Log("Player is jumping while not grounded and touching a wall");
            }
            
            jump = false;
        }

        if ((playerController.IsTouchingWallOnLeft || playerController.IsTouchingWallOnRight))
        {
            playerController.Rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;
        }
        else
        {
            playerController.Rigidbody.constraints = RigidbodyConstraints2D.None;
        }

        if(direction.x == 0)
        {
            direction.x = horizontalMovement;
        }
        
        return direction;
    }
}