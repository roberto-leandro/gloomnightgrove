using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Defines the player's movements mechanics, including character switching, jumping, wall jumping, and all the button input reading necessary.
/// </summary>
public class PlayerController : AbstractController
{
    // State info
    private bool isCrowActive = true; // true for crow, false for cat
    private bool isDoublejumpAvailable;
    public bool IsDoublejumpAvailable { get { return isDoublejumpAvailable; } set { isDoublejumpAvailable = value; } }
    private Collision2D enemyCollision;
    public bool CollidedWithEnemy { get { return enemyCollision != null; } set { if (!value) { enemyCollision = null; } } }
    public Collision2D EnemyCollision { get { return enemyCollision; } }
    [SerializeField] private int healthPoints;

    // The controls inputted by the player
    protected bool jump;
    public bool Jump { get { return jump; } set { jump = value; } }
    protected float horizontalMovement;
    public float HorizontalMovement { get { return horizontalMovement; } }
    private bool switchAnimal;

    // State variables to wall jump hitstun
    private bool wallHitstunDirection; // true for right, false for left
    public bool WallHitstunDirection { get { return wallHitstunDirection; } set { wallHitstunDirection = value; } }
    private int wallHitstunCounter = 0;
    public int WallHitstunCounter { get { return wallHitstunCounter; } set { wallHitstunCounter = value; } }

    // State variables to handle enemy hitstun
    private bool enemyHitstunDirection; // true for right, false for left
    public bool EnemyHitstunDirection { get { return enemyHitstunDirection; } set { enemyHitstunDirection = value; } }
    private int enemyHitstunCounter = 0;
    public int EnemyHitstunCounter { get { return enemyHitstunCounter; } set { enemyHitstunCounter = value; } }

    // Enemy knockback tweaks
    [SerializeField] private float enemyKnockbackUpwardsForce;
    public float EnemyKnockbackUpwardsForce { get { return enemyKnockbackUpwardsForce; } }
    [SerializeField] private float enemyKnockbackSidewaysForce;
    public float EnemyKnockbackSidewaysForce { get { return enemyKnockbackSidewaysForce; } }
    [SerializeField] private int enemyHitstunDuration;
    public int EnemyHitstunDuration { get { return enemyHitstunDuration; } }
    [SerializeField] private float moveInfluenceAfterEnemyKnockback;
    public float MoveInfluenceAfterEnemyKnockback { get { return moveInfluenceAfterEnemyKnockback; } }

    // Wall jumping tweaks
    [SerializeField] private float wallJumpUpwardsForce;
    public float WallJumpUpwardsForce { get { return wallJumpUpwardsForce; }  }
    [SerializeField] private float wallJumpSidewaysForce;
    public float WallJumpSidewaysForce { get { return wallJumpSidewaysForce; } }
    [SerializeField] private int walljumpMovementDuration;
    public int WalljumpMovementDuration { get { return walljumpMovementDuration; } }
    [SerializeField] private float moveInfluenceAfterWalljump;
    public float MoveInfluenceAfterWalljump { get { return moveInfluenceAfterWalljump; } }

    // Cache Unity objects that are used frequently to avoid getting them every time
    protected Animator animator;
    protected Collider2D characterCollider;
    public Collider2D CharacterCollider { get { return characterCollider; } }
    [SerializeField]  protected Text healthText;

    // Start is called before the first frame update.
    public override void Start()
    {
        // Call parent to initialize all the necessary stuff
        base.Start();

        // Unity stuff
        animator = GetComponent<Animator>();
        characterCollider = GetComponent<Collider2D>();

        // Custom stuff
        movementStrategy = new PlayerCrowMovementStrategy(this); // Default animal is crow
        UpdateHealthText();
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
    /// Changes the current animal, which involves three operations:
    /// 1. Change the player's sprite.
    /// 2. Change the movement strategy.
    /// 3. Flip the crowIsActive boolean.
    /// </summary>
    void SwitchAnimal()
    {
        //TODO code to change sprite
        // Change movement strategy and sprite, according to the currently active animal.
        if (isCrowActive)
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
        isCrowActive = !isCrowActive;
    }

    /// <summary>
    /// Handle animal switching and the setting of animation parameters in FixedUpdate().
    /// </summary>
    protected override void AdditionalFixedUpdateOperations()
    {
        if (switchAnimal)
        {
            SwitchAnimal();
            switchAnimal = false;
        }

        // Set the parameters for our animation
        animator.SetFloat("Speed", Mathf.Abs(rigidBody.velocity.x));
    }

    /// <summary>
    /// We override the abstract controller's way of handling a ground collision so we can refund the player's double jump.
    /// </summary>
    /// <param name="collision"></param>
    protected override void OnGroundCollisionEnter(Collision2D collision)
    {
        base.OnGroundCollisionEnter(collision);
        isDoublejumpAvailable = true;
    }

    /// <summary>
    /// Take damage and move the player away from the enemy.
    /// </summary>
    protected override void OnEnemyCollisionEnter(Collision2D collision)
    {
        healthPoints = healthPoints - 1;
        if(healthPoints > 0)
        {
            UpdateHealthText();
            enemyCollision = collision;
        }
        else
        {
            // TODO handle DEATH
            healthText.text = "u died ):";
        }
    }

    private void UpdateHealthText() 
    {
        healthText.text = "Health: " + healthPoints.ToString();
    }
}
