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

    // State info
    private bool crowIsActive = true; // crow if true and cat if false.
    private bool grounded;
    private bool touchingWall;
    private bool doublejumpAvailable;

    // Properties to access the variables
    public bool IsGrounded { get { return grounded; } set { grounded = value; } }
    public bool IsTouchingWall { get { return touchingWall; } set { touchingWall = value; } }
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
        if (Input.GetButton("SwitchAnimal"))
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
        } else
        {
            movementStrategy = new PlayerCrowMovementStrategy(this);
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
            Debug.Log("player was grounded");
        }
    }
}
