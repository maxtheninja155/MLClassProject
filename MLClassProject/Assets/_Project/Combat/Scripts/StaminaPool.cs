using UnityEngine;

namespace BossFight.Combat
{
    /// <summary>Plain stamina math with no Unity lifecycle, so it can be unit tested. Wrapped by <see cref="Stamina"/>.</summary>
    public class StaminaPool
    {
        public float Max { get; }
        public float Current { get; private set; }
        public float RegenPerSecond { get; }
        /// <summary>Seconds after spending before regen starts again.</summary>
        public float RegenDelay { get; }
        public float Normalized => Max > 0f ? Current / Max : 0f;

        float delayRemaining;

        public StaminaPool(float max, float regenPerSecond, float regenDelay)
        {
            Max = Mathf.Max(0f, max);
            Current = Max;
            RegenPerSecond = Mathf.Max(0f, regenPerSecond);
            RegenDelay = Mathf.Max(0f, regenDelay);
        }

        public bool CanAfford(float cost) => Current >= cost;

        /// <summary>Spends if affordable and restarts the regen delay. Returns false and spends nothing otherwise.</summary>
        public bool TrySpend(float cost)
        {
            if (cost < 0f || !CanAfford(cost)) return false;
            Current -= cost;
            delayRemaining = RegenDelay;
            return true;
        }

        /// <summary>Advance time. Regen only runs once the delay has fully elapsed.</summary>
        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f) return;
            if (delayRemaining > 0f)
            {
                delayRemaining -= deltaTime;
                if (delayRemaining > 0f) return;
                deltaTime = -delayRemaining;   // only the part of this tick after the delay ended regenerates
                delayRemaining = 0f;
            }
            Current = Mathf.Min(Max, Current + RegenPerSecond * deltaTime);
        }

        public void Refill()
        {
            Current = Max;
            delayRemaining = 0f;
        }
    }
}
