using UnityEngine;

public class MarkovMovementStrategy : AbstractMovementStrategy<KromavController>
{
    public MarkovMovementStrategy(KromavController controller) : base(controller) { }

    public override Vector2 DetermineMovement()
    {
        Vector2 direction = new Vector2();

        // Resolve jumping
        if (characterController.Jump)
        {

            if (characterController.IsGrounded)
            {
                // Jump if the player is grounded
                direction.y = characterController.JumpForce;
                characterController.Jump = false;
            }
        }

        return direction;
    }
}
