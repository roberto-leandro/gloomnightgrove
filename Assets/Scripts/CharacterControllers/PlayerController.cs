using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : AbstractController
{
    // State info
    private bool crowIsActive = true; // crow if true and cat if false.
    private bool touchingWallOnLeft;
    private bool touchingWallOnRight;
    private bool doublejumpAvailable;

    // Used to store contacts when detecting a collision reusing the array generated less work for c#'s garbage collector
    ContactPoint2D[] collisionContacts;

    Dictionary<GameObject, int> currentCollisions; // 0 right wall, 1 left wall, 2 ground

    // Properties to access the variables
    public bool IsTouchingWallOnLeft { get { return touchingWallOnLeft; } set { touchingWallOnLeft = value; } }
    public bool IsTouchingWallOnRight { get { return touchingWallOnRight; } set { touchingWallOnRight = value; } }
    public bool IsDoublejumpAvailable { get { return doublejumpAvailable; } set { doublejumpAvailable = value; } }
    
    // The controls inputted by the player
    protected bool jump;
    protected float horizontalMovement;
    private bool switchAnimal;

    public bool Jump { get { return jump; } set { jump = value; } }
    public float HorizontalMovement { get { return horizontalMovement; } }

    // Wall jumping tweaks
    [SerializeField] private float wallJumpUpwardsForce;
    public float WallJumpUpwardsForce { get { return wallJumpUpwardsForce; }  }
    [SerializeField] private float wallJumpSidewaysForce;
    public float WallJumpSidewaysForce { get { return wallJumpSidewaysForce; } }
    [SerializeField] private int walljumpMovementDuration = 25;
    public int WalljumpMovementDuration { get { return walljumpMovementDuration; } }

    private BoxCollider2D CharacterCollider;

    // Start is called before the first frame update.
    public new void Start()
    {
        // Initialize variables that will be used later on.
        rigidBody = GetComponent<Rigidbody2D>();
        movementStrategy = new PlayerCrowMovementStrategy(this); // Default animal is crow
        CharacterCollider = GetComponent<BoxCollider2D>();
        collisionContacts = new ContactPoint2D[2];
        currentCollisions = new Dictionary<GameObject, int>();
    }

    // Update is called once per frame
    void Update()
	{
        // Read player each update 
        ReadPlayerInput();
    }

    /// <summary>
    /// Reads player input and sets the appropriate info for the movement startegy to use.
    /// </summary>
    public void ReadPlayerInput()
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
            Debug.Log("Cat selected");
        } else
        {
            movementStrategy = new PlayerCrowMovementStrategy(this);
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
    /// From an array containing the contact points of a collision, determines if said collision grounded the player.
    /// For this, it is assumed that the array contains two items, as the player uses a box collider and all terrain is a flat, non-sloped surface.
    /// If the y value of both points is the same and the x values are different, then the collision must have happened on the ground.
    /// </summary>
    /// <param name="collisionPoints">a array containing the two contact points involved in a player-terrain collision.</param>
    /// <returns>True if the collision occurred against the ground, false otherwise.</returns>
    private bool IsCollisionGround(ContactPoint2D[] collisionPoints)
    {
        return (collisionPoints[0].point.y == collisionPoints[1].point.y &&
                collisionPoints[0].point.x != collisionPoints[1].point.x);
    }

    /// <summary>
    /// From an array containing the contact points of a collision, determines if said collision was against a wall.
    /// For this, it is assumed that the array contains two items, as the player uses a box collider and all terrain is a flat, non-sloped surface.
    /// If the x value of both points is the same and the y values are different, then the collision must have happened on the ground.
    /// Also, if the x value of the points is less than the player's center, it must be a left wall, and a right wall if it is greater.
    /// </summary>
    /// <param name="collisionPoints">a array containing the two contact points involved in a player-terrain collision.</param>
    /// <returns>-1 if the collision was not againt a wall, 0 if it was a right wall, 1 if it was a left wall. </returns>
    private int IsCollisionWall(ContactPoint2D[] collisionPoints)
    {
        if (collisionPoints[0].point.x == collisionPoints[1].point.x &&
                collisionPoints[0].point.y != collisionPoints[1].point.y)
        {
            // find the wall's direction
            if (collisionPoints[0].point.x < CharacterCollider.bounds.center.x)
            {
                // left wall
                return 1;
            } else
            {
                // right wall
                return 0;
            }
        } else
        {
            // not a wall collision
            return -1;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle terrain collisions
        if (collision.gameObject.CompareTag("Terrain"))
        {
            // Because we are using a box collider, we will always get two contacts
            int contactCount = collision.GetContacts(collisionContacts);
            if(contactCount != 2)
            {
                Debug.Log("A terrain collision with contacts != 2 occured. If you are seeing this printed in runtime, please tell Roberto he is stupid.");
            }

            // Handle wall collisions
            int isWall = IsCollisionWall(collisionContacts);
            if(isWall == 0)
            {
                // Wall is to the right
                touchingWallOnRight = true;
                currentCollisions.Add(collision.gameObject, 0);
                Debug.Log("Player is touching a wall on the right");

            } else if(isWall == 1) {
                // Wall is to the left
                touchingWallOnLeft = true;
                currentCollisions.Add(collision.gameObject, 1);
                Debug.Log("Player is touching a wall on the left");
            }
            else
            {
                // Handle non-wall terrain collisions
                if (IsCollisionGround(collisionContacts))
                {
                    grounded = true;
                    doublejumpAvailable = true;
                    currentCollisions.Add(collision.gameObject, 2);
                    Debug.Log("Player is grounded");
                }
            }
            // TODO edge cases if both points are the same (I have verified this can happen)

        } else if (collision.gameObject.CompareTag("Enemy"))
        {
            // TODO Handle enemy collisions
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        // Handle terrain collisions
        if (collision.gameObject.CompareTag("Terrain"))
        {
            // Look for the object in our current collisions dictionary
            int collisionType = 0;
            bool collisionExists = currentCollisions.TryGetValue(collision.gameObject, out collisionType);
            if(!collisionExists)
            {
                // If the collision does not exists something has gone terribly wrong
                Debug.Log("WHAT THE FUCK WHY NOT");
            } else if(collisionType == 0)
            {
                // Collision was a right wall
                touchingWallOnRight = false;
                Debug.Log("Player is NOT touching a wall on the right");
            }
            else if (collisionType == 1)
            {
                // Collision was a left wall
                touchingWallOnLeft = false;
                Debug.Log("Player is NOT touching a wall on the left");
            }
            else if (collisionType == 2)
            {
                // Collision was ground
                grounded = false;
                Debug.Log("Player is NOT grounded");
            }

            // Remove the collision from the dictionary, as it is not longer colliding
            currentCollisions.Remove(collision.gameObject);
        }
    }

}
