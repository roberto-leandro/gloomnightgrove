using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : AbstractController
{
    // State info
    private bool crowIsActive = true; // true for crow, false for cat
    private bool doublejumpAvailable;
    public bool IsDoublejumpAvailable { get { return doublejumpAvailable; } set { doublejumpAvailable = value; } }

    // The controls inputted by the player
    protected bool jump;
    public bool Jump { get { return jump; } set { jump = value; } }
    protected float horizontalMovement;
    public float HorizontalMovement { get { return horizontalMovement; } }
    private bool switchAnimal;

    // State variables to handle wall jumps
    private bool lastWalljumpDirection; // true for right, false for left
    public bool LastWalljumpDirection { get { return lastWalljumpDirection; } set { lastWalljumpDirection = value; } }
    private int lastWalljumpCounter = 0;
    public int LastWalljumpCounter { get { return lastWalljumpCounter; } set { lastWalljumpCounter = value; } }

    // Wall jumping tweaks
    [SerializeField] private float wallJumpUpwardsForce;
    public float WallJumpUpwardsForce { get { return wallJumpUpwardsForce; }  }
    [SerializeField] private float wallJumpSidewaysForce;
    public float WallJumpSidewaysForce { get { return wallJumpSidewaysForce; } }
    [SerializeField] private int walljumpMovementDuration = 25;
    public int HinderedMovementAfterWalljumpDuration { get { return walljumpMovementDuration; } }
    [SerializeField] private float moveInfluenceAfterWalljump = 0.25f;
    public float MoveInfluenceAfterWalljump { get { return moveInfluenceAfterWalljump; } }

    // Cache Unity objects that are used frequently to avoid getting them every time
    private BoxCollider2D characterCollider;

    // Start is called before the first frame update.
    public new void Start()
    {
        // Initialize variables that will be used later on.
        // Unity objects
        rigidBody = GetComponent<Rigidbody2D>();
        characterCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        collisionContactPoints = new ContactPoint2D[2];

        // Custom stuff
        movementStrategy = new PlayerCrowMovementStrategy(this); // Default animal is crow
    }

    // Update is called once per frame
    void Update()
	{
        // Read player input each update 
        ReadPlayerInput();
    }

    /// <summary>
    /// Reads player input and sets the appropriate info for the movement startegy to use.
    /// </summary>
    private void ReadPlayerInput()
    {
        // Determine horizontal movement
        // We use Input.GetAxisRaw to avoid Unity's automatic smoothing to enable the player to stop on a dime
        // Multiply the input by our movement speed to allow controller users to input analog movement 
        horizontalMovement = Input.GetAxisRaw("Horizontal") * movementSpeed;

        // Determine if player wants to jump
        // We only want to change jump if it is already false, changing its value when its true can result in missed inputs
        if (!jump)
        {
            jump = Input.GetButtonDown("Jump");
        }

        // Check if the current animal should be switched, using the same method as with jumps
        if (!switchAnimal)
        {
            switchAnimal = Input.GetButtonDown("SwitchAnimal");
        }
        
    }

    /// <summary>
    /// Changes the current animal, whoch involves three operations:
    /// 1. Change the player's sprite.
    /// 2. Change the movement strategy.
    /// 3. Flip the crowIsActive boolean.
    /// </summary>
    void SwitchAnimal()
    {
        //TODO code to change sprite
        // Change movement strategy and sprite, according to the currently active animal.
        if (crowIsActive)
        {
            movementStrategy = new PlayerCatMovementStrategy(this);
            spriteRenderer.color = new Color32(255, 175, 255, 255);
            Debug.Log("Cat selected");
        } else
        {
            movementStrategy = new PlayerCrowMovementStrategy(this);
            spriteRenderer.color = Color.black;
            Debug.Log("Crow selected");
        }
        
        // Flip the currently active animal boolean.
        crowIsActive = !crowIsActive;
    }

    /// <summary>
    /// Handle animal switching in FixedUpdate().
    /// </summary>
    protected override void AdditionalFixedUpdateOperations()
    {
        if (switchAnimal)
        {
            SwitchAnimal();
            switchAnimal = false;
        }
    }

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
    void ParseTerrainCollisionContactPoints(Collision2D collision, out bool isGround, out bool isLeftWall, out bool isRightWall)
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
    /// 
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
                // Left wall
                currentLeftWall = collision.gameObject;
                Debug.Log("Player is touching a wall on the left");

            }
            else if (isRightWall)
            {
                // Right wall
                currentRightWall = collision.gameObject;
                Debug.Log("Player is touching a wall on the right");
            }

            // Check y for ground
            if (isGround)
            {
                doublejumpAvailable = true;
                currentGround = collision.gameObject;
                Debug.Log("Player is grounded");
            }

        } else if (collision.gameObject.CompareTag("Enemy"))
        {
            // TODO Handle enemy collisions
        }
    }

    /// <summary>
    /// Called by Unity whenever a collision ends.
    /// If the collision is against terrain, we check whether it is ground or a wall and set the appropriate state variable to null.
    /// 
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit2D(Collision2D collision)
    {

        // Handle terrain collisions
        if (collision.gameObject.CompareTag("Terrain"))
        {
            if(collision.gameObject == currentGround)
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

    /// <summary>
    /// Called by Unity on all frames between the ones where a collision started and ened.
    /// If the collision is against terrain, we check whether it is ground or a wall and set the appropriate state variable to the
    /// object the character collided against.
    /// 
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
                // Left wall
                currentLeftWall = collision.gameObject;

            }
            else if (isRightWall)
            {
                // Right wall
                currentRightWall = collision.gameObject;
            }
            
            if (isGround)
            {
                doublejumpAvailable = true;
                currentGround = collision.gameObject;
            }
        }

    }
}
