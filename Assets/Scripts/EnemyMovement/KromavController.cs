using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KromavController : EnemyController
{
    // The controls inputted by the player
    protected bool bite;

    protected bool jump;
    public bool Jump { get { return jump; } set { jump = value; } }

    // Attack hitboxes
    [SerializeField] protected Collider2D biteCollider;
    [SerializeField] protected List<Collider2D> spikeColliders;

    public override void Start()
    {
        // Call parent to initialize all the necessary stuff
        base.Start();

        switch (movementType)
        {
            case EnemyMovementStrategy.BackAndForth:
                movementStrategy = new BackAndForthMovementStrategy(this);
                break;

            case EnemyMovementStrategy.Markov:
                movementStrategy = new MarkovMovementStrategy(this);
                break;

            case EnemyMovementStrategy.NoMovement:
                movementStrategy = new NoMovementStrategy(this);
                break;
        }

        // Disable all attack hitboxes
        biteCollider.enabled = false;
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

        // Determine if player wants to jump
        // We only want to change jump if it is already false, changing its value when its true can result in missed inputs
        if (!jump)
        {
            jump = Input.GetButtonDown("Jump");
        }

        if (!bite)
        {
            bite = Input.GetButtonDown("SwitchAnimal");
        }
    }

    /// <summary>
    /// Handle animal switching and the setting of animation parameters in FixedUpdate().
    /// </summary>
    protected override void AdditionalFixedUpdateOperations()
    {
        // Set the parameters for our animation
        animator.SetFloat("HorizontalSpeed", Mathf.Abs(rigidBody.velocity.x));
        animator.SetFloat("VerticalSpeed", rigidBody.velocity.y);
        animator.SetBool("Jump", jump);
        animator.SetBool("Bite", bite);

        if(bite)
        {
            BiteAttack();
        }
    }

    private void BiteAttack()
    {
        biteCollider.enabled = true;

        bite = false;
    }
}
