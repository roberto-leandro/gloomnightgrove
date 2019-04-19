using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : AbstractController
{
    // The controls inputted by the player
    protected bool jump;
    protected float horizontalMovement;
    
    // State info
    private bool crowIsActive = true; // crow if true and cat if false.
    private bool doublejumpAvailable;

    // Properties to access the variables
    public bool Jump { get { return jump; } set { jump = value; } }
    public float HorizontalMovement { get { return horizontalMovement; } }
    public bool IsDoublejumpAvailable { get { return doublejumpAvailable; } set { doublejumpAvailable = value; } }

    // Start is called before the first frame update.
    public new void Start()
    {
        // Initialize variables that will be used later on.
        rigidBody = GetComponent<Rigidbody2D>();
        movementStrategy = new PlayerCrowMovementStrategy(this); // Default animal is crow
    }

    // Update is called once per frame
    void Update()
	{
        // Check if the current animal should be switched
        if (Input.GetButton("SwitchAnimal"))
        {
            SwitchAnimal();
        }

        // Read player input here 
        DeterminePlayerInput();
    }

    /// <summary>
    /// Reads player input and sets the appropriate info for the movement startegy to use.
    /// </summary>
    public void DeterminePlayerInput()
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
