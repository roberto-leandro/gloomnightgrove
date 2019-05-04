using UnityEngine;

/// <summary>
/// Defines how objects in the game can be moved in the x and y axis.
/// </summary>
interface IMovable
{
    void Move(Vector2 direction);
}