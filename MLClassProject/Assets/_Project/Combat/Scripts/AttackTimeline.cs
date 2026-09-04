using System;
using BossFight.Core;

namespace BossFight.Combat
{
    public enum AttackPhase { Idle, Windup, Active, Recovery }

    /// <summary>
    /// Plain attack timeline math with no Unity lifecycle, so it can be unit tested. Wrapped by <see cref="AttackRunner"/>.
    /// Ticked with fixed steps, so phase lengths are identical at any time scale.
    /// </summary>
    public class AttackTimeline
    {
        public AttackPhase Phase { get; private set; } = AttackPhase.Idle;
        public AttackData Attack { get; private set; }
        public float TimeInPhase { get; private set; }
        public bool IsBusy => Phase != AttackPhase.Idle;

        /// <summary>The attack and the phase just entered. Idle means it ended.</summary>
        public event Action<AttackData, AttackPhase> PhaseEntered;
        /// <summary>The attack that ended, and whether it was cut short.</summary>
        public event Action<AttackData, bool> Finished;

        public bool Start(AttackData attack)
        {
            if (attack == null || IsBusy) return false;
            Attack = attack;
            TimeInPhase = 0f;
            Enter(AttackPhase.Windup);
            return true;
        }

        public void Interrupt()
        {
            if (!IsBusy) return;
            var cut = Attack;
            Attack = null;
            TimeInPhase = 0f;
            Enter(AttackPhase.Idle);
            Finished?.Invoke(cut, true);
        }

        /// <summary>Advance by one step. Leftover time carries into the next phase, so nothing drifts.</summary>
        public void Tick(float deltaTime)
        {
            if (!IsBusy || deltaTime <= 0f) return;
            TimeInPhase += deltaTime;
            while (IsBusy && TimeInPhase >= Duration(Phase))
            {
                TimeInPhase -= Duration(Phase);
                Advance();
            }
        }

        float Duration(AttackPhase phase)
        {
            switch (phase)
            {
                case AttackPhase.Windup: return Attack.WindupSeconds;
                case AttackPhase.Active: return Attack.ActiveSeconds;
                case AttackPhase.Recovery: return Attack.RecoverySeconds;
                default: return float.PositiveInfinity;
            }
        }

        void Advance()
        {
            switch (Phase)
            {
                case AttackPhase.Windup: Enter(AttackPhase.Active); break;
                case AttackPhase.Active: Enter(AttackPhase.Recovery); break;
                case AttackPhase.Recovery:
                    var done = Attack;
                    Attack = null;
                    TimeInPhase = 0f;
                    Enter(AttackPhase.Idle, done);
                    Finished?.Invoke(done, false);
                    break;
            }
        }

        void Enter(AttackPhase phase, AttackData attack = null)
        {
            Phase = phase;
            PhaseEntered?.Invoke(attack ?? Attack, phase);
        }
    }
}
