using System;
using BossFight.Core;
using UnityEngine;

namespace BossFight.Combat
{
    /// <summary>
    /// Hit points for a body. Put it on the body root.
    /// Raises <see cref="FightEvents.OnHit"/> on every landed hit and <see cref="FightEvents.OnDeath"/> once.
    /// Bodies call <see cref="GrantInvulnerability"/> at the start of a roll to open an i-frame window.
    /// </summary>
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField, Min(1f)] float max = 100f;

        HealthPool pool;
        float invulnerableUntil = -1f;

        public HealthPool Pool => pool ??= new HealthPool(max);
        public float Max => Pool.Max;
        public float Current => Pool.Current;
        public float Normalized => Pool.Normalized;
        public bool IsDead => Pool.IsDead;
        public bool IsInvulnerable => Time.time < invulnerableUntil;

        /// <summary>A hit landed on this body.</summary>
        public event Action<DamageInfo> Damaged;
        /// <summary>A hit was ignored because of i-frames (a roll).</summary>
        public event Action<DamageInfo> Dodged;
        public event Action Died;

        void Awake() => pool ??= new HealthPool(max);

        public void GrantInvulnerability(float seconds) =>
            invulnerableUntil = Mathf.Max(invulnerableUntil, Time.time + seconds);

        public void ResetToFull()
        {
            Pool.Refill();
            invulnerableUntil = -1f;
        }

        public void TakeDamage(DamageInfo info)
        {
            if (IsDead) return;
            if (IsInvulnerable)
            {
                Dodged?.Invoke(info);
                return;
            }

            bool died = Pool.Damage(info.Amount);
            if (info.Amount <= 0f) return;

            Damaged?.Invoke(info);
            FightEvents.RaiseHit(info.Source, gameObject, info);

            if (died)
            {
                Died?.Invoke();
                FightEvents.RaiseDeath(gameObject);
            }
        }
    }
}
