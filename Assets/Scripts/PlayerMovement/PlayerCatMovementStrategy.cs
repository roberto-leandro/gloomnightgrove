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
                // Jump if the player is grounded 
                direction.y = playerController.JumpForce;
                
            }
            else if(playerController.IsTouchingWallOnLeft || playerController.IsTouchingWallOnRight)
            {
                // If the player is touching a wall they are allowed to wall jump

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
                
                Debug.Log("Player is jumping while not grounded and touching a wall");
            }
            
            jump = false;
        }

        if(direction.x == 0)
        {
            direction.x = horizontalMovement;
        }
        
        return direction;
    }
}