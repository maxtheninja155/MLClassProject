namespace BossFight.Core
{
    /// <summary>Anything that can be hit: the player, the boss, a training dummy.</summary>
    public interface IDamageable
    {
        void TakeDamage(DamageInfo info);
    }
}
