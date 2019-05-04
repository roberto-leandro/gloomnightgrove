using UnityEngine;

public class MarkovMovementStrategy : AbstractMovementStrategy<EnemyController>
{
    public MarkovMovementStrategy(EnemyController controller) : base(controller) { }

    public override Vector2 DetermineMovement()
    {
        throw new System.NotImplementedException();
    }
}
