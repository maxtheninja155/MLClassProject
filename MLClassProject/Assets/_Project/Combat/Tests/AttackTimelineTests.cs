using System.Collections.Generic;
using BossFight.Core;
using NUnit.Framework;
using UnityEngine;

namespace BossFight.Combat.Tests
{
    public class AttackTimelineTests
    {
        const float Step = 0.02f;   // the project's fixed timestep

        static AttackData Attack(float windup, float active, float recovery)
        {
            var a = ScriptableObject.CreateInstance<AttackData>();
            a.WindupSeconds = windup;
            a.ActiveSeconds = active;
            a.RecoverySeconds = recovery;
            return a;
        }

        /// <summary>Ticks until the phase changes; returns how many steps it took.</summary>
        static int StepsUntilPhaseChanges(AttackTimeline t)
        {
            var start = t.Phase;
            int steps = 0;
            while (t.Phase == start && steps < 10000) { t.Tick(Step); steps++; }
            return steps;
        }

        [Test]
        public void StartEntersWindupAndReportsBusy()
        {
            var t = new AttackTimeline();
            Assert.IsTrue(t.Start(Attack(0.3f, 0.15f, 0.4f)));
            Assert.AreEqual(AttackPhase.Windup, t.Phase);
            Assert.IsTrue(t.IsBusy);
        }

        [Test]
        public void CannotStartWhileBusy()
        {
            var t = new AttackTimeline();
            t.Start(Attack(0.3f, 0.15f, 0.4f));
            Assert.IsFalse(t.Start(Attack(0.1f, 0.1f, 0.1f)));
        }

        [Test]
        public void PhaseLengthsMatchTheDataToWithinOneStep()
        {
            var t = new AttackTimeline();
            t.Start(Attack(0.3f, 0.15f, 0.4f));
            Assert.AreEqual(15, StepsUntilPhaseChanges(t), 1, "windup 0.3 s = 15 steps");
            Assert.AreEqual(AttackPhase.Active, t.Phase);
            Assert.AreEqual(8, StepsUntilPhaseChanges(t), 1, "active 0.15 s = 7.5 steps");
            Assert.AreEqual(AttackPhase.Recovery, t.Phase);
            Assert.AreEqual(20, StepsUntilPhaseChanges(t), 1, "recovery 0.4 s = 20 steps");
            Assert.AreEqual(AttackPhase.Idle, t.Phase);
        }

        [Test]
        public void LargeStepCarriesOverAndStillEntersEveryPhase()
        {
            var t = new AttackTimeline();
            var entered = new List<AttackPhase>();
            t.PhaseEntered += (a, p) => entered.Add(p);
            t.Start(Attack(0.1f, 0.1f, 0.1f));
            t.Tick(1f);   // one big step swallows the whole attack
            CollectionAssert.AreEqual(new[] { AttackPhase.Windup, AttackPhase.Active, AttackPhase.Recovery, AttackPhase.Idle }, entered);
            Assert.IsFalse(t.IsBusy);
        }

        [Test]
        public void FinishedFiresOnceWithInterruptedFalse()
        {
            var t = new AttackTimeline();
            int count = 0; bool interrupted = true; AttackData finished = null;
            t.Finished += (a, i) => { count++; interrupted = i; finished = a; };
            var attack = Attack(0.1f, 0.1f, 0.1f);
            t.Start(attack);
            for (int i = 0; i < 100; i++) t.Tick(Step);
            Assert.AreEqual(1, count);
            Assert.IsFalse(interrupted);
            Assert.AreSame(attack, finished);
        }

        [Test]
        public void InterruptReturnsToIdleAndReportsTheAttack()
        {
            var t = new AttackTimeline();
            AttackData reported = null; bool interrupted = false; AttackPhase lastEntered = AttackPhase.Windup;
            t.Finished += (a, i) => { reported = a; interrupted = i; };
            t.PhaseEntered += (a, p) => lastEntered = p;
            var attack = Attack(0.3f, 0.15f, 0.4f);
            t.Start(attack);
            t.Tick(Step);
            t.Interrupt();
            Assert.AreEqual(AttackPhase.Idle, t.Phase);
            Assert.AreEqual(AttackPhase.Idle, lastEntered);
            Assert.AreSame(attack, reported);
            Assert.IsTrue(interrupted);
            Assert.IsTrue(t.Start(attack), "can start again right after an interrupt");
        }

        [Test]
        public void ZeroLengthPhasesAreSkippedOnTheNextTick()
        {
            var t = new AttackTimeline();
            t.Start(Attack(0f, 0f, 0.1f));
            t.Tick(Step);
            Assert.AreEqual(AttackPhase.Recovery, t.Phase);
        }

        [Test]
        public void TickWhileIdleDoesNothing()
        {
            var t = new AttackTimeline();
            t.Tick(1f);
            Assert.AreEqual(AttackPhase.Idle, t.Phase);
            Assert.AreEqual(0f, t.TimeInPhase);
        }
    }
}
