using System;
using System.Collections;
using BossFight.Core;
using UnityEngine;

namespace BossFight.Combat
{
    public enum AttackPhase { Idle, Windup, Active, Recovery }

    /// <summary>
    /// Plays an <see cref="AttackData"/> timeline: windup (the telegraph), active (hitbox live), recovery (punish window).
    /// Put it on the body root. Bodies call <see cref="TryStart"/>; it refuses while busy or when stamina is short.
    /// Uses scaled time, so it runs correctly at training time scales.
    /// </summary>
    public class AttackRunner : MonoBehaviour
    {
        [Tooltip("Hitbox armed during the active window unless TryStart is given another one.")]
        [SerializeField] Hitbox defaultHitbox;
        [Tooltip("Optional. When set, attacks spend stamina and are refused when it is short.")]
        [SerializeField] Stamina stamina;

        Coroutine routine;
        Hitbox activeHitbox;
        float phaseStart;

        public AttackPhase Phase { get; private set; } = AttackPhase.Idle;
        public AttackData Current { get; private set; }
        public bool IsBusy => Phase != AttackPhase.Idle;
        public float TimeInPhase => Time.time - phaseStart;

        public event Action<AttackData> Started;
        public event Action<AttackData, AttackPhase> PhaseChanged;
        /// <summary>Attack finished, or was interrupted (second argument true).</summary>
        public event Action<AttackData, bool> Finished;

        void Reset()
        {
            defaultHitbox = GetComponentInChildren<Hitbox>();
            stamina = GetComponent<Stamina>();
        }

        public bool CanStart(AttackData data) =>
            data != null && !IsBusy && (stamina == null || stamina.CanAfford(data.StaminaCost));

        public bool TryStart(AttackData data, Hitbox hitbox = null)
        {
            if (!CanStart(data)) return false;
            if (stamina != null) stamina.TrySpend(data.StaminaCost);
            Current = data;
            routine = StartCoroutine(Run(data, hitbox != null ? hitbox : defaultHitbox));
            return true;
        }

        /// <summary>Stops the current attack immediately (stagger, death, episode reset).</summary>
        public void Interrupt()
        {
            if (!IsBusy) return;
            if (routine != null) StopCoroutine(routine);
            if (activeHitbox != null) activeHitbox.Disarm();
            activeHitbox = null;
            var interrupted = Current;
            Current = null;
            SetPhase(AttackPhase.Idle);
            Finished?.Invoke(interrupted, true);
        }

        IEnumerator Run(AttackData data, Hitbox hitbox)
        {
            Started?.Invoke(data);

            SetPhase(AttackPhase.Windup);
            yield return new WaitForSeconds(data.WindupSeconds);

            SetPhase(AttackPhase.Active);
            activeHitbox = hitbox;
            if (hitbox != null) hitbox.Arm(data, gameObject);
            yield return new WaitForSeconds(data.ActiveSeconds);
            if (hitbox != null) hitbox.Disarm();
            activeHitbox = null;

            SetPhase(AttackPhase.Recovery);
            yield return new WaitForSeconds(data.RecoverySeconds);

            var done = Current;
            Current = null;
            routine = null;
            SetPhase(AttackPhase.Idle);
            Finished?.Invoke(done, false);
        }

        void SetPhase(AttackPhase phase)
        {
            Phase = phase;
            phaseStart = Time.time;
            PhaseChanged?.Invoke(Current, phase);
        }

        void OnDisable() => Interrupt();
    }
}
