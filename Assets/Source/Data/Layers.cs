using UnityEngine;

public static class Layers
{
    public static LayerMask Environment { get; } = LayerMask.GetMask("Environment"); 
    public static LayerMask EnemyHurtBox { get; } = LayerMask.GetMask("EnemyHurtBox", "EnemyProjectile");
}