using UnityEngine;

/// <summary>
/// Defines the default behavior of all characters in the game, including enemies and the player.
/// </summary>
public abstract class AbstractController : MonoBehaviour, IMovable
{

    // Cache unity's rigidbody object so we don't need to get it every time
    protected Rigidbody2D rigidBody;
    public float XVelocity { get { return rigidBody.velocity.x; } }
    public float YVelocity { get { return rigidBody.velocity.y; } }
    protected SpriteRenderer spriteRenderer;

    // A strategy that will define how each concrete controller moves the character
    public IMovementStrategy movementStrategy;

    // How fast the character moves
    [SerializeField] protected float movementSpeed;
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
    // Current walls and ground are stored to be able to handle them properly on collision exits
    [SerializeField] protected GameObject currentGround;
    public bool IsGrounded { get { return currentGround != null; } }
    protected GameObject currentLeftWall;
    public bool IsTouchingWallOnLeft { get { return currentLeftWall != null; } }
    protected GameObject currentRightWall;
    public bool IsTouchingWallOnRight { get { return currentRightWall != null; } }

    protected bool isFacingRight = true;
    public bool IsFacingRight { get { return isFacingRight; } }

    // Used to store contacts when detecting a collision, as reusing the same array generates less work for c#'s garbage collector
    protected ContactPoint2D[] collisionContactPoints;
    public ContactPoint2D[] CollisionContactPoints { get { return collisionContactPoints; } }

    // Start is called before the first frame update.
    public virtual void Start()
    {
        // Initialize variables that will be used later on.
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collisionContactPoints = new ContactPoint2D[4];
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
        if (direction.y == 0)
        {
            direction.y = rigidBody.velocity.y;
        }

        // Calculate fall multiplier
        direction = DetermineFallMultiplier(direction);

        // Move by setting the rigidbody's velocity
        rigidBody.velocity = direction;
    }

    /// <summary>
    /// Checks if the character's direction should be flipped by comparing the character's movement direction and the direction currently being faced.
    /// </summary>
    protected void ManageFlip() {
        if ((rigidBody.velocity.x > 0.01 && !isFacingRight) || (rigidBody.velocity.x < -0.01 && isFacingRight))
        {
            Flip();
        }
    }

    /// <summary>
    /// Flips the character. 
    /// The flip is performed by negating the character's rigidbody.transform.LocalScale value, so that both the sprite and the colllider are flipped.
    /// </summary>
    protected void Flip()
    {
        rigidBody.transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        isFacingRight = !isFacingRight;
    }

    /// <summary>
    /// If the <c>fallMultiplier</c> flag is true, adds a multiplier to the gravity acting on a falling character.
    /// This allows jumps to have a heavier, more satisfying feel.
    /// </summary>
    /// <param name="direction">The original direction where the character is moving. The multiplier is only applied if the movement on the y axis is less than 0 (i.e. the character is moving down).</param>
    /// <returns>a Vector2 object with the approrpiate multiplier applied.</returns>
    protected Vector2 DetermineFallMultiplier(Vector2 direction)
    {
        if(applyFallMultiplier && !IsGrounded && direction.y < 0)
        {
            // Add our fall multiplier minus one as unity already applied the multiplier one time
            direction.y += Physics2D.gravity.y * (fallMultiplier-1);  
        }
        return direction;
    }

    /// <summary>
    /// Default implementation of FixedUpdate for all characters. It simply uses the current movementStrategy to figure out where to go and
    /// calls Move() in that direction.
    /// This method is written as the highest form of abstraction for updating all characters in the game, under the
    /// assumption that every character takes some sort of decision on where to move and then performs that movement.
    /// If this does not work for some characters it can be overrridden in their controller, but this should be avoided.
    /// </summary>
    private void FixedUpdate()
    {
        AdditionalFixedUpdateOperations();
        Vector2 movementDirection = movementStrategy.DetermineMovement();
        Move(movementDirection);
        ManageFlip();
    }

    /// <summary>
    /// This method may be used by subclasses to perform operations during fixed update other than moving, such as switching animal for the player character.
    /// This allows us add custom operations for each controller in FixedUpdate without overriding FixedUpdate(), as its only implementation should be in this class.
    /// </summary>
    protected virtual void AdditionalFixedUpdateOperations(){}

    /// <summary>
    /// Iterates through a Collision2D's list of ContactPoint2D objects, checking the normal of each point to determine what type of
    /// terrain the character is touching.
    /// Because we are using a box collider, the following logic is used:
    ///     normal.x ==  1 -> ground collision
    ///     normal.y ==  1 -> right wall collision
    ///     normal.y == -1 -> left wall collisiojn
    /// 
    /// When a type of collision is determined, we store the game object collided against in the appropriate variable (currentGround, currentLeftWall or currentRightWall).
    /// </summary>
    /// <param name="collision"></param>
    protected void ParseTerrainCollisionContactPoints(Collision2D collision, out bool isGround, out bool isLeftWall, out bool isRightWall)
    {
        // Get the contact points from the collision object, store it in the collisionContactPoints array
        int contactCount = collision.GetContacts(collisionContactPoints);

        isGround = false;
        isLeftWall = false;
        isRightWall = false;

        // Parse the points to determine what type of terrain is being collided against
        foreach (ContactPoint2D point in collisionContactPoints)
        {
            // Check the point's normals to determine the type of collision
            // Only overwrite each bool of it wasn't already true
            isLeftWall = isLeftWall || point.normal.x == 1;
            isRightWall = isRightWall || point.normal.x == -1;
            isGround = isGround || point.normal.y == 1;
        }
    }

    /// <summary>
    /// Called by Unity whenever a collision occurs.
    /// If the collision is against terrain, we check whether it is ground or a wall and set the appropriate state variable to the game 
    /// object the character collided against.
    /// TODO enemy collisions.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle terrain collisions
        if (collision.gameObject.CompareTag("Terrain"))
        {
            ParseTerrainCollisionContactPoints(collision, out bool isGround, out bool isLeftWall, out bool isRightWall);

            if (isLeftWall)
            {
                OnLeftWallCollisionEnter(collision);
            }

            if (isRightWall)
            {
                OnRightWallCollisionEnter(collision);
            }
            
            if (isGround)
            {
                OnGroundCollisionEnter(collision);
            }

        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            OnEnemyCollisionEnter(collision);
        }
    }
    
    /// <summary>
    /// Called by Unity on all frames between the ones where a collision started and ened.
    /// If the collision is against terrain, we check whether it is ground or a wall and set the appropriate state variable to the
    /// object the character collided against.
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionStay2D(Collision2D collision)
    {
        // Handle terrain collisions
        if (collision.gameObject.CompareTag("Terrain"))
        {
            ParseTerrainCollisionContactPoints(collision, out bool isGround, out bool isLeftWall, out bool isRightWall);

            if (isLeftWall)
            {
                OnLeftWallCollisionStay(collision);
            }
            else if (IsTouchingWallOnLeft && currentLeftWall == collision.gameObject)
            {
                currentLeftWall = null;
            }

            if (isRightWall)
            {
                OnRightWallCollisionStay(collision);
            }
            else if (IsTouchingWallOnRight && currentRightWall == collision.gameObject)
            {
                currentRightWall = null;
            }

            if (isGround)
            {
                OnGroundCollisionStay(collision);
            }
        }
    }

    /// <summary>
    /// Called by Unity whenever a collision ends.
    /// If the collision is against terrain, we check whether it is ground or a wall and set the appropriate state variable to null.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit2D(Collision2D collision)
    {

        // Handle terrain collisions
        if (collision.gameObject.CompareTag("Terrain"))
        {
            if (collision.gameObject == currentGround)
            {
                // Collision was ground
                currentGround = null;
                Debug.Log("Player is NOT grounded");
            }
            else if (collision.gameObject == currentLeftWall)
            {
                // Collision was a left wall
                currentLeftWall = null;
                Debug.Log("Player is NOT touching a wall on the left");
            }
            else if (collision.gameObject == currentRightWall)
            {
                // Collision was a right wall
                currentRightWall = null;
                Debug.Log("Player is NOT touching a wall on the right");
            }
        }
    }

    protected virtual void OnGroundCollisionEnter(Collision2D collision)
    {
        currentGround = collision.gameObject;
        Debug.Log("Player is grounded");
    }

    protected virtual void OnEnemyCollisionEnter(Collision2D collision){}

    protected virtual void OnGroundCollisionStay(Collision2D collision)
    {
        currentGround = collision.gameObject;
    }

    protected virtual void OnRightWallCollisionEnter(Collision2D collision)
    {
        currentRightWall = collision.gameObject;
        Debug.Log("Player is touching a wall on the right");
    }

    protected virtual void OnRightWallCollisionStay(Collision2D collision)
    {
        currentRightWall = collision.gameObject;
        Debug.Log("Player is touching a wall on the right STAY");
    }

    protected virtual void OnLeftWallCollisionEnter(Collision2D collision)
    {
        currentLeftWall = collision.gameObject;
        Debug.Log("Player is touching a wall on the left");
    }

    protected virtual void OnLeftWallCollisionStay(Collision2D collision)
    {
        currentLeftWall = collision.gameObject;
    }
}
