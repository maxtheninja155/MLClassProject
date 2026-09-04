using UnityEngine;

namespace BossFight.Combat
{
    /// <summary>
    /// Stamina for a body. Attacks and rolls spend it, it regenerates after a short delay.
    /// <see cref="AttackRunner"/> spends it automatically when present on the same object.
    /// </summary>
    public class Stamina : MonoBehaviour
    {
        [SerializeField, Min(0f)] float max = 100f;
        [SerializeField, Min(0f)] float regenPerSecond = 20f;
        [SerializeField, Min(0f)] float regenDelay = 1f;

        StaminaPool pool;

        public StaminaPool Pool => pool ??= new StaminaPool(max, regenPerSecond, regenDelay);
        public float Max => Pool.Max;
        public float Current => Pool.Current;
        public float Normalized => Pool.Normalized;

        void Awake() => pool ??= new StaminaPool(max, regenPerSecond, regenDelay);
        void Update() => Pool.Tick(Time.deltaTime);

        public bool CanAfford(float cost) => Pool.CanAfford(cost);
        public bool TrySpend(float cost) => Pool.TrySpend(cost);
        public void ResetToFull() => Pool.Refill();
    }
}
