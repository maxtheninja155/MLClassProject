using UnityEngine;

namespace BossFight.Combat
{
    /// <summary>Plain hit-point math with no Unity lifecycle, so it can be unit tested. Wrapped by <see cref="Health"/>.</summary>
    public class HealthPool
    {
        public float Max { get; }
        public float Current { get; private set; }
        public float Normalized => Max > 0f ? Current / Max : 0f;
        public bool IsDead => Current <= 0f;

        public HealthPool(float max)
        {
            Max = Mathf.Max(1f, max);
            Current = Max;
        }

        /// <summary>Applies damage. Returns true if this call killed the pool (only ever once).</summary>
        public bool Damage(float amount)
        {
            if (IsDead || amount <= 0f) return false;
            Current = Mathf.Max(0f, Current - amount);
            return IsDead;
        }

        public void Refill() => Current = Max;
    }
}
