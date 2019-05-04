using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements movement common to all animals, like movement in the x axis.
/// All player movement strategies must extend this class.
/// </summary>
public abstract class AbstractPlayerMovementStrategy : AbstractMovementStrategy<PlayerController> 
{
    public AbstractPlayerMovementStrategy(PlayerController controller) : base(controller) { }

    /// <summary>
    /// Define this method to comply with the IMovementInterface; use the abstract keyword to leave the implementation up to subclasses. 
    /// </summary>
    public override abstract Vector2 DetermineMovement();

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
        if (characterController.LastWalljumpCounter == 0)
        {
            // If not, move normally
            direction.x += characterController.HorizontalMovement;
        }
        else
        {
            // Otherwise, these are the moments just after a wall jump
            // Substract one from the last wall jump counter
            characterController.LastWalljumpCounter--;

            // Keep moving player away from the wall
            if (characterController.LastWalljumpDirection)
            {
                direction.x = characterController.WallJumpSidewaysForce;
            }
            else
            {
                direction.x = characterController.WallJumpSidewaysForce * -1;
            }

            // Add player input, taking into account how much they can influece their direction after a walljump
            direction.x += characterController.HorizontalMovement * characterController.MoveInfluenceAfterWalljump;
        }

        return direction;
    }

}
