using UnityEngine;

/// <summary>Defines the default behavior of all characters in the game, including enemies and the player.</summary>
public abstract class AbstractController : MonoBehaviour, IMovable
{
    // Cache unity's rigidbody object so we don't need to get it every time
    [SerializeField] protected Rigidbody2D rigidBody;

    // A strategy that will define how each concrete controller moves the character
    [SerializeField] protected IMovementStrategy movementStrategy;

    // How fast the character moves
    [SerializeField] protected float movementSpeed = 200;
    public float MovementSpeed { get { return movementSpeed; } }

    // How high the character jumps
    [SerializeField] private float jumpForce;
    public float JumpForce { get { return jumpForce; } }

    // A multiplier that accelerates the character when falling
    // This causes jumps to feel nicer as it takes more frames to go up then to get back down
    [SerializeField] private float fallMultiplier = 1.03f;
    // Indicates if the fall multiplier should be applied
    [SerializeField] private bool applyFallMultiplier = false;

    // State variables common to most characters
    protected bool grounded;
    public bool IsGrounded { get { return grounded; } set { grounded = value; } }
    protected bool touchingWall;
    public bool IsTouchingWall { get { return touchingWall; } set { touchingWall = value; } }

    // Start is called before the first frame update.
    public void Start()
    {
        // Initialize variables that will be used later on.
        rigidBody = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Helper method that is the only place where forces should be applied to the character's Rigidbody.
    /// Should only be called in a FixedUpdate() method.
    /// This method assumes the x axis represents "normal" walking movement, and the y axis represents a jump.
    /// If this assumption is not correct for a given character, this method can be overridden in their controller.
    /// </summary>
    /// <param name="direction">Vector2 direction: the direction where the character will move.</param>
    public void Move(Vector2 direction)
    {
        // In order to move we change the velocity of our rididBody

        // Multiply the x velocity by Time.fixedDeltaTime so our physics are not tied to the machine's framerate
        direction.x = direction.x * Time.fixedDeltaTime;

        // If no jump is to be performed, we should keep the rigidbody's original velocity
        if(direction.y == 0)
        {
            direction.y = rigidBody.velocity.y;
        }

        // Calculate fall multiplier
        direction = DetermineFallMultiplier(direction);

        // Move by setting the rigidbody's velocity
        rigidBody.velocity = direction;
    }

    /// <summary>
    /// If the <c>fallMultiplier</c> flag is true, adds a multiplier to the gravity acting on a falling character.
    /// This allows jumps to have a heavier, more satisfying feel.
    /// </summary>
    /// <param name="direction">The original direction where the character is moving. The multiplier is only applied if the movement on the y axis is less than 0 (i.e. the character is moving down).</param>
    /// <returns>a Vector2 object with the approrpiate multiplier applied.</returns>
    protected Vector2 DetermineFallMultiplier(Vector2 direction)
    {
        if(applyFallMultiplier && direction.y < 0)
        {
            // Add our fall multiplier minus one as unity already applied the multiplier one time
            direction.y += Physics2D.gravity.y * (fallMultiplier-1);  
        }
        return direction;
    }

    /// <summary>
    /// Default implementation of FixedUpdate for all characters. It simply uses the current movementStrategy to figure out where to go and
    /// calls Move().
    /// This method is written as thew highest form of abstraction for updating all characters in the game, under the
    /// assumption that every character takes some sort of decision on where to move and then performs that movement.
    /// If this does not work for some characters it can be overrridden in their controller, but this should be avoided.
    /// </summary>
    private void FixedUpdate()
    {
        Vector2 movementDirection = movementStrategy.DetermineMovement();
        Move(movementDirection);
    }
    
}
