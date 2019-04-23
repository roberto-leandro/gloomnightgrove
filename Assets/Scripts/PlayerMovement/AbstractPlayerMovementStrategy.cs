using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPlayerMovementStrategy : IMovementStrategy
{
    // We keep a reference to the player controller so the concrete strategies can obtain the state information they need
    protected PlayerController playerController;

    public AbstractPlayerMovementStrategy(PlayerController controller)
    {
        playerController = controller;
    }
    
    /// <summary>
    /// Define this method to comply with the IMovementInterface; use the abstract keyword to leave the implementation up to subclasses. 
    /// </summary>
    public abstract Vector2 DetermineMovement();

    /// <summary>
    /// Horizontal movement is determined the same way in either strategy, so we generalize it here and call it in each concrete
    /// implementation of DetermineMovement().
    /// 
    /// </summary>
    /// <returns></returns>
    protected Vector2 DetermineHorizontalMovement()
    {
        Vector2 direction = new Vector2(); 
        
        // Check if the player wall jumped recently
        if (playerController.LastWalljumpCounter == 0)
        {
            // If not, move normally
            direction.x += playerController.HorizontalMovement;
        }
        else
        {
            // Otherwise, these are the moments just after a wall jump
            // Substract one from the last wall jump counter
            playerController.LastWalljumpCounter--;

            // Keep moving player away from the wall
            if (playerController.LastWalljumpDirection)
            {
                direction.x = playerController.WallJumpSidewaysForce;
            }
            else
            {
                direction.x = playerController.WallJumpSidewaysForce * -1;
            }

            // Add player input, taking into account how much they can influece their direction after a walljump
            direction.x += playerController.HorizontalMovement * playerController.MoveInfluenceAfterWalljump;
        }

        return direction;
    }

}
