using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines state variables all enemies have in common.
/// All enemy controllers must extend this class.
/// </summary>
public class EnemyController : AbstractController
{
    [SerializeField] protected EnemyMovementStrategy movementType = EnemyMovementStrategy.NoMovement;
    [SerializeField] protected Animator animator;
    public Animator Animator { get { return animator; } }

    // Start is called before the first frame update
    public override void Start()
	{
        // Call parent to initialize all the necessary stuff
        base.Start();
        
        switch(movementType)
        {
            case EnemyMovementStrategy.BackAndForth:
                movementStrategy = new BackAndForthMovementStrategy(this);
                break;

            case EnemyMovementStrategy.NoMovement:
                movementStrategy = new NoMovementStrategy(this);
                break;
        }
    }
	
	// Update is called once per frame
	void Update()
	{
        animator.SetFloat("Speed", Mathf.Abs(rigidBody.velocity.x));
    }
    
}

public enum EnemyMovementStrategy 
{
    BackAndForth,
    Markov,
    NoMovement
};