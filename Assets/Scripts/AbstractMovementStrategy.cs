using UnityEngine;

/// <summary>
/// Abstract class that defines every movement strategy as a generic class of type AbstractController.
/// All concrete movement strategies in the game must extend this class. By doing this we guarantee that every strategy
/// will have a pointer to the controller using it, so it can obtain all the state information it requires.
/// </summary>
/// <typeparam name="ControllerType">An AbstractController object that uses this strategy to mdetermine its movement.</typeparam>
public abstract class AbstractMovementStrategy<ControllerType> : IMovementStrategy where ControllerType : AbstractController 
{
    // We keep a reference to the character controller so the concrete strategies can obtain the state information they need
    protected ControllerType characterController;

    public AbstractMovementStrategy(ControllerType controller)
    {
        characterController = controller;
    }

    // Define the method to comply with the IMovable interface; it will be implemented in subclasses.
    abstract public Vector2 DetermineMovement();
}
