using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : AbstractController
{

    // Tweaks to player movement
    [SerializeField] private float movementSpeed;
    public float MovementSpeed { get { return movementSpeed; } }
    [SerializeField] private float jumpForce;
    public float JumpForce { get { return jumpForce; } }
    public Rigidbody2D Rigidbody { get { return rigidBody; } }

    // State info
    private bool crowIsActive = false; // crow if true and cat if false.
    private bool grounded;
    private bool touchingWallOnLeft;
    private bool touchingWallOnRight;
    private bool doublejumpAvailable;

    // Properties to access the variables
    public bool IsGrounded { get { return grounded; } set { grounded = value; } }
    public bool IsTouchingWallOnLeft { get { return touchingWallOnLeft; } set { touchingWallOnLeft = value; } }
    public bool IsTouchingWallOnRight { get { return touchingWallOnRight; } set { touchingWallOnRight = value; } }
    public bool IsDoublejumpAvailable { get { return doublejumpAvailable; } set { doublejumpAvailable = value; } }

    // Start is called before the first frame update.
    public void Start()
    {
        // Initialize variables that will be used later on.
        rigidBody = GetComponent<Rigidbody2D>();
        movementStrategy = new PlayerCrowMovementStrategy(this);
    }

    // Update is called once per frame
    void Update()
	{
        // Check if the current animal should be switched
        if (Input.GetButtonDown("SwitchAnimal"))
        {
            SwitchAnimal();
        }

        ((AbstractPlayerMovementStrategy)movementStrategy).DeterminePlayerInput();
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

    void OnCollisionEnter2D(Collision2D collider)
    {
        // Check if the player touched the ground
        if (collider.gameObject.CompareTag("Ground"))
        {
            // Set the player as grounded and refund their doublejump
            grounded = true;
            doublejumpAvailable = true;
            Debug.Log("Player is grounded");
        }

        // Check if the player touched a wall
        if (collider.gameObject.CompareTag("Wall"))
        {
            // Center point of the wall
            Vector3 colliderCenter = collider.collider.bounds.center;

            // Point of contact between the player and the wall
            Vector3 colliderContactPoint = collider.contacts[0].point;
            

            // See if the player is touching a wall on its left or on its right
            if (colliderCenter.x < colliderContactPoint.x)
            {
                // Wall is on the left
                touchingWallOnLeft = true;
                Debug.Log("Player is touching a wall on its left");
            }
            else if(colliderCenter.x > colliderContactPoint.x)
            {
                // Wall is on the right
                touchingWallOnRight = true;
                Debug.Log("Player is touching a wall on its right");
            }
            
        }
        
    }

    private void OnCollisionExit2D(Collision2D collider)
    {
        if (collider.gameObject.CompareTag("Ground"))
        {
            // Player is not grounded
            grounded = false;
            Debug.Log("Player is not grounded");
        }

        if (collider.gameObject.CompareTag("Wall"))
        {
            // Player is not touching a wall
            touchingWallOnLeft = false;
            touchingWallOnRight = false;
            Debug.Log("Player stopped touching a wall");
        }
    }
}
