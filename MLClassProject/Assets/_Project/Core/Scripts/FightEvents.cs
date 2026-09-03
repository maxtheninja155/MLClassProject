using System;
using UnityEngine;

namespace BossFight.Core
{
    /// <summary>
    /// The two things everyone needs to know about: someone got hit, someone died.
    /// Combat raises them. The arena's fight manager and the RL reward listen.
    /// </summary>
    public static class FightEvents
    {
        /// <summary>attacker, victim, damage</summary>
        public static event Action<GameObject, GameObject, DamageInfo> OnHit;

        /// <summary>victim</summary>
        public static event Action<GameObject> OnDeath;

        public static void RaiseHit(GameObject attacker, GameObject victim, DamageInfo info) => OnHit?.Invoke(attacker, victim, info);
        public static void RaiseDeath(GameObject victim) => OnDeath?.Invoke(victim);
    }
}
