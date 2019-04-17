using UnityEngine;

/// <summary>Defines the default behavior of all characters in the game, including enemies and the player.</summary>
public abstract class AbstractController : MonoBehaviour, IMovable
{
    [SerializeField] protected IMovementStrategy movementStrategy;
    [SerializeField] protected Rigidbody2D rigidBody;

    // Start is called before the first frame update.
    public void Start()
    {
        // Initialize variables that will be used later on.
        rigidBody = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Helper method that is the only place where forces should be applied to the character's Rigidbody.
    /// Should only be called in a FixedUpdate() method.
    /// </summary>
    /// </param>Vector2 direction: the direction where the character's transform should be moved to.</param>
    public void Move(Vector2 direction)
    {
        // In order to move we change the velocity of our rididBody

        // Multiply the x velocity by Time.fixedDeltaTime so our phisics our not tied to the machine's framerate
        direction.x = direction.x * Time.fixedDeltaTime;

        // If no jump is to be performed, we should keep the rigidbody's original velocity
        if(direction.y == 0)
        {
            direction.y = rigidBody.velocity.y;
        }

        rigidBody.velocity = direction;
    }

    /// <summary>
    /// Default implementation of FixedUpdate for all characters. It simply uses the current movementStrategy to figure out where to go and
    /// calls Move().
    /// This might be sufficient for some characters, and need to be overridden for others.
    /// </summary>
    private void FixedUpdate()
    {
        Vector2 movementDirection = movementStrategy.DetermineMovement();
        
        Move(movementDirection);
    }
    
}
