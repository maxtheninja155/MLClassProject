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
        Health health;

        void Reset() => runner = GetComponent<AttackRunner>();

        public void Configure(AttackRunner attackRunner, AttackData attackData, float seconds)
        {
            runner = attackRunner;
            attack = attackData;
            everySeconds = seconds;
        }
        void Awake() => health = GetComponent<Health>();

        // FixedUpdate, not Update: swing times then depend on game time only, not on frame size.
        void FixedUpdate()
        {
            if (runner == null || attack == null || Time.time < next) return;
            if (health != null && health.IsDead) return;
            if (runner.TryStart(attack)) next = Time.time + everySeconds;
        }
    }
}
