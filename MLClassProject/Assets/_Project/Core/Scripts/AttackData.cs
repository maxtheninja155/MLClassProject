using UnityEngine;

namespace BossFight.Core
{
    /// <summary>
    /// Timing and numbers for one attack. Combat runs it; Player and Boss author assets from it.
    /// Windup is the telegraph, Active is when the hitbox is live, Recovery is the punish window.
    /// </summary>
    [CreateAssetMenu(menuName = "BossFight/Attack Data", fileName = "Attack_")]
    public class AttackData : ScriptableObject
    {
        public string DisplayName = "Attack";

        [Header("Timing (seconds)")]
        [Min(0f)] public float WindupSeconds = 0.4f;
        [Min(0f)] public float ActiveSeconds = 0.15f;
        [Min(0f)] public float RecoverySeconds = 0.5f;

        [Header("Numbers")]
        [Min(0f)] public float Damage = 10f;
        [Min(0f)] public float StaminaCost = 15f;
        [Min(0f)] public float Range = 2f;

        public float TotalSeconds => WindupSeconds + ActiveSeconds + RecoverySeconds;
    }
}
