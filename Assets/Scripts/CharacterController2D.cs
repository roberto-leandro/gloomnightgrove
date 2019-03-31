using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;

    // Determining ground and ceiling
    [SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching
    const float k_GroundedRadius = .2f;                                         // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;                                                    // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f;                                          // Radius of the overlap circle to determine if the player can stand up

    // Determining walls
    [SerializeField] private LayerMask m_WhatIsWall;							// A mask determining what is a wall to the character
    [SerializeField] private Transform m_LeftWallCheck;                         // A position marking where to check for left walls 
    [SerializeField] private Transform m_RightWallCheck;					    // A position marking where to check for right walls
    const float k_WallslidingRadius = .2f;                                      // Radius of the overlap circle to determine if wall sliding
    private bool m_Wallsliding;                                                 // Whether or not the player is wall sliding.

    private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;                  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

    private bool m_HasDoublejump = true;               // Determines is player can double jump

    // Sprite swapping
    private SpriteRenderer m_Renderer;                                            // Unity object to render sprites
    private Sprite m_ClemmSprite, m_UltharSprite;                                   // Sprites the player can use
    private bool m_ClemmIsActive = true;                                        // true for Clemm, false for ulthar 

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

    private void Start()
    {
        // Get the object's SpriteRenderer instance
        m_Renderer = GetComponent<SpriteRenderer>();

        // Load the sprites from the resources folder
        m_UltharSprite = Resources.Load<Sprite>("ulthar-idle");
        m_ClemmSprite = Resources.Load<Sprite>("clemm-idle");

        // Set Clemm as the renderer's sprite
        m_Renderer.sprite = m_ClemmSprite;
    }

	private void FixedUpdate()
	{
        // Handle ground
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        if (CheckCollisions(colliders))
        {
            m_Grounded = true;

            if (!wasGrounded)
            {
                m_HasDoublejump = true; // Refund double jump if player lands after not being grounded
                OnLandEvent.Invoke();
            }
        }

        // Handle wall sliding if not grounded
        if (!m_Grounded)
        {
            // Check right
            colliders = Physics2D.OverlapCircleAll(m_RightWallCheck.position, k_WallslidingRadius, m_WhatIsWall);
            m_Wallsliding = CheckCollisions(colliders);

            // If there is no right wall, check left
            if (!m_Wallsliding)
            {
                colliders = Physics2D.OverlapCircleAll(m_LeftWallCheck.position, k_WallslidingRadius, m_WhatIsWall);
                m_Wallsliding = CheckCollisions(colliders);
            }
        }


    }

    // Returns true if there is at least one collision in a Collider2D array
    private bool CheckCollisions(Collider2D[] colliders)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }


	public void Move(float move, bool crouch, bool jump)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}

		// If the player should jump...
        if (jump)
        {
            if (m_Grounded)
            {
                // Player can jump anytime if grounded
                Jump();
                print("NORMAL JUMPED");
            } else if (m_Wallsliding && m_ClemmIsActive)
            {
                // Player can wall jump only if Clemm is active
                Jump();

                // Wall jumping refunds double jump
                m_HasDoublejump = true;

                print("WALL JUMPED");
            }
            else if (!m_Grounded && !m_Wallsliding && !m_ClemmIsActive && m_HasDoublejump)
            {
                // Player can jump while airborne only if they have a doublejump and Clemm is not active
                Jump();

                // Their double jump is subsequently lost
                m_HasDoublejump = false;
                
                print("DOUBLE JUMPED");
            }
        }
		
	}

    // Switched the player object's current sprite between Clemm and Ulthar
    public void SwitchSprite()
    {
        if(m_ClemmIsActive)
        {
            // Deactivate Clemm and switch sprite to Ulthar
            m_ClemmIsActive = false;
            m_Renderer.sprite = m_UltharSprite;
        } else
        {
            // Activate Clemm and switch sprite to Clemm
            m_ClemmIsActive = true;
            m_Renderer.sprite = m_ClemmSprite;
        }
    }

    // Performs a jmup by adding a vertical force to the player
    private void Jump()
    {
        m_Grounded = false;
 
        // Cancel current vertical momentum, as it will cancel out the jump if player is falling
        m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        
    }

    

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}