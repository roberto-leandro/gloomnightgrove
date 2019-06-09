using System.Collections;
using UnityEngine;

public class MarkovMovementStrategy : AbstractMovementStrategy<KromavController>
{

    public MarkovMovementStrategy(KromavController controller) : base(controller) { }

    private MarkovChain markovChain = new MarkovChain(new float[,] {
        { 0.00f, 0.30f, 0.40f, 0.30f}, // MoveToPlayer
        { 0.80f, 0.05f, 0.10f, 0.05f}, // Jump 
        { 0.80f, 0.05f, 0.05f, 0.10f}, // Bite
        { 0.80f, 0.05f, 0.10f, 0.05f}  // Spike
    }, 0);

    private MarkovMoves currentMove;
    private Transform playerPosition = GameObject.FindWithTag("Player").transform;

    public override Vector2 DetermineMovement()
    {
        Vector2 direction = new Vector2();

        if (characterController.IsIdle && !characterController.WalkingToPlayer)
        {
            // Set not idle
            characterController.IsIdle = false;

            // Generate next action
            currentMove = (MarkovMoves)markovChain.generateNextState();
            Debug.Log("generated " + currentMove);

            // Act on the generated action
            switch (currentMove)
            {
                case MarkovMoves.Jump:
                    characterController.Jump = true;
                    if (characterController.IsGrounded)
                    {
                        // Jump if the player is grounded
                        direction.y = characterController.JumpForce;
                    }
                    break;

                case MarkovMoves.Bite:
                    characterController.Bite = true;
                    break;

                case MarkovMoves.Spikes:
                    characterController.Spikes = true;
                    break;

                case MarkovMoves.MoveToPlayer:
                    characterController.WalkToPlayer();
                    break;
            }
        }
        
         // If markov was moving towards the player, keep moving
         if(currentMove == MarkovMoves.MoveToPlayer && characterController.WalkingToPlayer)
         {
            if(playerPosition.position.x > characterController.transform.position.x)
            {
                direction.x += characterController.MovementSpeed;
            } else
            {
                direction.x += -characterController.MovementSpeed;
            }
        }

        return direction;
    }
}

enum MarkovMoves
{
    MoveToPlayer = 0,
    Jump = 1,
    Bite = 2,
    Spikes = 3
}
