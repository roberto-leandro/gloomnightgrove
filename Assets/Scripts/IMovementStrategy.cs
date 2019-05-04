using UnityEngine;

/// <summary>
/// Defines how controllers can use a strategy object to determine its character's movement.
/// </summary>
public interface IMovementStrategy
{
    Vector2 DetermineMovement();
}
