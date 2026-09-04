namespace BossFight.Core
{
    /// <summary>
    /// The boss's move list. PLACEHOLDER: these names are stand-ins until the team picks the real moves.
    /// The RL agent's action space is the size of this enum, so change it here and nowhere else.
    /// </summary>
    public enum BossMove
    {
        None = 0,
        Advance,
        Retreat,
        LightSwing,
        HeavySlam,
        Sweep,
        Lunge,
    }
}
