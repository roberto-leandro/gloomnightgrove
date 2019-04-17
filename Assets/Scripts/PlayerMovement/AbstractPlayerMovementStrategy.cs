using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPlayerMovementStrategy : IMovementStrategy
{
    // We keep a reference to the player controller so the concrete strategies can obtain the state information they need
    protected PlayerController playerController;

    // The controls inputted by the player
    protected bool jump;
    protected float horizontalMovement;

    public AbstractPlayerMovementStrategy(PlayerController controller)
    {
        playerController = controller;
    }

    public void DeterminePlayerInput()
    {
        // Determine horizontal movement
        float horizontalInput = Input.GetAxis("Horizontal");
        if(horizontalInput < 0)
        {
            horizontalMovement = playerController.MovementSpeed * -1;
        } else if (horizontalInput > 0)
        {
            horizontalMovement = playerController.MovementSpeed;
        } else
        {
            horizontalMovement = 0;
        }

        // Determine if player wants to jump
        // We only want to change jump if it is already false, changing its value when its true can result in missed inputs
        if (!jump){
            jump = Input.GetButtonDown("Jump");
        }
    }
    
    /// <summary>
    /// Define this method to comply with the IMovementInterface; use the abstract keyword to leave the implementation up to subclasses. 
    /// </summary>
    public abstract Vector2 DetermineMovement();

}
