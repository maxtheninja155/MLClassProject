using BossFight.Core;
using UnityEngine;

namespace BossFight.Combat.Sandbox
{
    /// <summary>Sandbox only: fires an attack every few seconds until the body dies.</summary>
    public class AttackOnTimer : MonoBehaviour
    {
        [SerializeField] AttackRunner runner;
        [SerializeField] AttackData attack;
        [SerializeField, Min(0.1f)] float everySeconds = 2f;

        float next;

        void Reset() => runner = GetComponent<AttackRunner>();

        void Update()
        {
            if (runner == null || attack == null || Time.time < next) return;
            var health = GetComponent<Health>();
            if (health != null && health.IsDead) return;
            if (runner.TryStart(attack)) next = Time.time + everySeconds;
        }
    }
}
