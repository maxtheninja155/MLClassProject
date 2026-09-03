using UnityEngine;

namespace BossFight.Core
{
    /// <summary>One hit. Combat fills this in; whatever gets hit receives it.</summary>
    public struct DamageInfo
    {
        public float Amount;
        public GameObject Source;

        public DamageInfo(float amount, GameObject source)
        {
            Amount = amount;
            Source = source;
        }
    }
}
