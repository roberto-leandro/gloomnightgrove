using UnityEngine;

public class NoMovementStrategy : AbstractMovementStrategy<EnemyController>
{
    public NoMovementStrategy(EnemyController controller) : base(controller) { }

    public override Vector2 DetermineMovement()
    {
        return new Vector2(0, 0);
    }
}
