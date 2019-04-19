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

}
