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
    public override Vector2 DetermineMovement() {
        Vector2 direction = new Vector2();

        // If the player collided with an enemy, handle that first and ignore inputs for this frame
        if(characterController.CollidedWithEnemy)
        {
            // Set up enemy knockback depending on whether the enemy is to the left or right
            if(characterController.CharacterCollider.bounds.center.x <= characterController.EnemyCollision.otherCollider.bounds.center.x)
            {
                characterController.EnemyHitstunDirection = false;
                direction += new Vector2(characterController.EnemyKnockbackSidewaysForce, characterController.EnemyKnockbackUpwardsForce);
            } else
            {
                characterController.EnemyHitstunDirection = true;
                direction += new Vector2(-1 * characterController.EnemyKnockbackSidewaysForce, characterController.EnemyKnockbackUpwardsForce);
            }

            // Set hitstun counter
            characterController.EnemyHitstunCounter = characterController.EnemyHitstunDuration;

            // Enemy collision handled, set to false
            characterController.CollidedWithEnemy = false;
        } else
        {
            // Calculate vertical and horizontal movement
            direction = DetermineVerticalMovement();
            direction += DetermineHorizontalMovement();
        }

        return direction;
    }

    /// <summary>
    /// Abstract method where each concrete strategy should implement its own jumping logic.
    /// </summary>
    /// <returns></returns>
    protected abstract Vector2 DetermineVerticalMovement(); 

    /// <summary>
    /// Horizontal movement is determined the same way in either strategy, so we generalize it here and call it in each concrete
    /// implementation of DetermineMovement().
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual Vector2 DetermineHorizontalMovement()
    {
        Vector2 direction = new Vector2(); 
        
        if(characterController.EnemyHitstunCounter > 0)
        {
            // Player is currently in enemy hitstun
            // Substract one from hitstun counter
            characterController.EnemyHitstunCounter--;

        // Keep moving player away from the enemy
        if (characterController.EnemyHitstunDirection)
        {
            direction.x = characterController.EnemyKnockbackSidewaysForce;
        }
        else
        {
            direction.x = characterController.EnemyKnockbackSidewaysForce * -1;
        }

        // Add player input, taking into account how much they can influece their direction after a walljump
        direction.x += characterController.HorizontalMovement * characterController.MoveInfluenceAfterEnemyKnockback;
        }
        else if (characterController.WallHitstunCounter > 0)
        {
            // Player is currently in wall jmup hitstun
            // Substract one from the last wall jump counter
            characterController.WallHitstunCounter--;

            // Keep moving player away from the wall
            if (characterController.WallHitstunDirection)
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
        else
        {
            // Player is not in hitstun, move normally
            direction.x += characterController.HorizontalMovement;
        }

        return direction;
    }

}
