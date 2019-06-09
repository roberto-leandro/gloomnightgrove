using System.Collections;
using UnityEngine;

public class KromavController : EnemyController
{
    // State variables controlled with markov chains
    protected bool bite;
    public bool Bite { get { return bite; } set { bite = value; } }

    protected bool spikes;
    public bool Spikes { get { return spikes; } set { spikes = value; } }

    protected bool jump;
    public bool Jump { get { return jump; } set { jump = value; } }

    [SerializeField] protected bool walkingToPlayer;
    public bool WalkingToPlayer { get { return walkingToPlayer; } set { walkingToPlayer = value; } }

    // Used to start new moves
    [SerializeField] protected bool isIdle;
    public bool IsIdle { get { return isIdle; } set { isIdle = value; } }

    // Determines how long kromav walks towards the player
    [SerializeField] private float walkToPlayerDuration;
    public float WalkToPlayerDuration { get { return walkToPlayerDuration; }  }

    // Attack hitboxes
    [SerializeField] protected Collider2D biteCollider;
    [SerializeField] protected Collider2D[] smallSpikeColliders;
    [SerializeField] protected Collider2D[] bigSpikeColliders;

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

        // The kromav sprite is looking the opposite way :P
        isFacingRight = !IsFacingRight;

        // Disable all attack hitboxes
        biteCollider.enabled = false;
        DisableSmallSpikeHitBox();
        DisableBigSpikeHitBox();
    }

    /// <summary>
    /// Reads player input and sets the appropriate info for the movement startegy to use.
    /// </summary>
    public void WalkToPlayer()
    {
        StartCoroutine(WalkTowardsPlayerCoroutine());
    }

    protected IEnumerator WalkTowardsPlayerCoroutine()
    {
        walkingToPlayer = true;
        yield return new WaitForSeconds(WalkToPlayerDuration);
        walkingToPlayer = false;
    }

    /// <summary>
    /// Handle animal switching and the setting of animation parameters in FixedUpdate().
    /// </summary>
    protected override void AdditionalFixedUpdateOperations()
    {
        // Set the parameters for our animation
        animator.SetFloat("Speed", Mathf.Abs(rigidBody.velocity.x));
        animator.SetFloat("VerticalSpeed", rigidBody.velocity.y);
        animator.SetBool("Jump", jump);

        animator.SetBool("Bite", bite);
        bite = false;
        
        animator.SetBool("Spikes", spikes);
        spikes = false;
    }

    private void EnableBiteHitbox()
    {
        biteCollider.enabled = true;
    }

    private void DisableBiteHitbox()
    {
        biteCollider.enabled = false;
    }

    private void EnableBigSpikeHitBox()
    {
        for (int i = 0; i < bigSpikeColliders.Length; i++)
        {
            bigSpikeColliders[i].enabled = true;
        }
    }

    private void DisableBigSpikeHitBox()
    {
        for (int i = 0; i < bigSpikeColliders.Length; i++)
        {
            bigSpikeColliders[i].enabled = false;
        }
    }
    private void EnableSmallSpikeHitBox()
    {
        for (int i = 0; i < smallSpikeColliders.Length; i++)
        {
            smallSpikeColliders[i].enabled = true;
        }
    }

    private void DisableSmallSpikeHitBox()
    {
        for (int i = 0; i < smallSpikeColliders.Length; i++)
        {
            smallSpikeColliders[i].enabled = false;
        }
    }

    private void SetIdle()
    {
        //Debug.Log("set idle true");
        isIdle = true;
    }

    private void SetJumpFalse()
    {
        //Debug.Log("set jump false");
        jump = false;
    }

    private void SetBiteFalse()
    {
        //Debug.Log("set bite false");
        bite = false;
    }

    private void SetSpikesFalse()
    {
        //Debug.Log("set spikes false");
        spikes = false;
    }
}

