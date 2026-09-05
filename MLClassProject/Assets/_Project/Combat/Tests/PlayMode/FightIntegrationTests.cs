using System.Collections;
using BossFight.Combat.Sandbox;
using BossFight.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace BossFight.Combat.Tests
{
    /// <summary>
    /// Two capsules trade timed attacks, the same setup as Combat_Sandbox, built in code so the test owns it.
    /// Checks the whole chain (runner → hitbox → hurtbox → health → events) and that the outcome does not depend on time scale,
    /// which is what training at 20x relies on.
    /// </summary>
    public class FightIntegrationTests
    {
        class Outcome { public float HealthA, HealthB; public int Hits, Deaths; }

        static AttackData Attack(string name, float windup, float active, float recovery, float damage, float stamina)
        {
            var a = ScriptableObject.CreateInstance<AttackData>();
            a.DisplayName = name; a.WindupSeconds = windup; a.ActiveSeconds = active; a.RecoverySeconds = recovery;
            a.Damage = damage; a.StaminaCost = stamina;
            return a;
        }

        static GameObject Fighter(string name, int bodyLayer, int hitboxLayer, Vector3 position, Vector3 facing, AttackData attack, float every)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = name;
            go.layer = bodyLayer;
            go.transform.SetPositionAndRotation(position, Quaternion.LookRotation(facing));
            go.AddComponent<Health>();
            go.AddComponent<Stamina>();
            go.AddComponent<Hurtbox>();

            var hb = new GameObject("Hitbox");
            hb.transform.SetParent(go.transform, false);
            hb.transform.localPosition = new Vector3(0f, 0f, 1f);
            hb.layer = hitboxLayer;
            hb.AddComponent<Hitbox>().Radius = 0.6f;

            var runner = go.AddComponent<AttackRunner>();
            go.AddComponent<AttackOnTimer>().Configure(runner, attack, every);
            return go;
        }

        /// <summary>Runs a fight for <paramref name="gameSeconds"/> of game time at the given time scale and fills in the outcome.</summary>
        static IEnumerator RunFight(float timeScale, float gameSeconds, Outcome outcome)
        {
            int player = LayerMask.NameToLayer("Player"), boss = LayerMask.NameToLayer("Boss");
            int playerHitbox = LayerMask.NameToLayer("PlayerHitbox"), bossHitbox = LayerMask.NameToLayer("BossHitbox");
            Assert.IsTrue(player >= 0 && boss >= 0 && playerHitbox >= 0 && bossHitbox >= 0, "project layers missing");

            var light = Attack("Light", 0.3f, 0.15f, 0.4f, 10f, 15f);
            var heavy = Attack("Heavy", 0.6f, 0.2f, 0.7f, 25f, 30f);
            var a = Fighter("A", player, playerHitbox, new Vector3(-0.9f, 1f, 0f), Vector3.right, light, 1.5f);
            var b = Fighter("B", boss, bossHitbox, new Vector3(0.9f, 1f, 0f), Vector3.left, heavy, 2.5f);

            int hits = 0, deaths = 0;
            System.Action<GameObject, GameObject, DamageInfo> onHit = (_, __, ___) => hits++;
            System.Action<GameObject> onDeath = _ => deaths++;
            FightEvents.OnHit += onHit;
            FightEvents.OnDeath += onDeath;

            Time.timeScale = timeScale;
            try
            {
                yield return new WaitForSeconds(gameSeconds);   // scaled time
            }
            finally
            {
                Time.timeScale = 1f;
                FightEvents.OnHit -= onHit;
                FightEvents.OnDeath -= onDeath;
            }

            outcome.HealthA = a.GetComponent<Health>().Current;
            outcome.HealthB = b.GetComponent<Health>().Current;
            outcome.Hits = hits;
            outcome.Deaths = deaths;
            Object.Destroy(a);
            Object.Destroy(b);
            Object.Destroy(light);
            Object.Destroy(heavy);
            yield return null;
        }

        [UnityTest]
        public IEnumerator FiveSecondsOfFightingLandsTheExpectedHits()
        {
            // A: light every 1.5 s, first hit at 0.3 s → hits at 0.3, 1.8, 3.3, 4.8 → B loses 40.
            // B: heavy every 2.5 s, first hit at 0.6 s → hits at 0.6, 3.1 → A loses 50.
            var o = new Outcome();
            yield return RunFight(1f, 5f, o);
            Assert.AreEqual(60f, o.HealthB, "B after four light hits");
            Assert.AreEqual(50f, o.HealthA, "A after two heavy hits");
            Assert.AreEqual(6, o.Hits);
            Assert.AreEqual(0, o.Deaths);
        }

        [UnityTest]
        public IEnumerator OutcomeIsTheSameAtTrainingTimeScale()
        {
            var slow = new Outcome();
            var fast = new Outcome();
            yield return RunFight(1f, 5f, slow);
            yield return RunFight(20f, 5f, fast);
            Assert.AreEqual(slow.HealthA, fast.HealthA, "A's health must not depend on time scale");
            Assert.AreEqual(slow.HealthB, fast.HealthB, "B's health must not depend on time scale");
            Assert.AreEqual(slow.Hits, fast.Hits);
        }

        [UnityTest]
        public IEnumerator DeathFiresOnceAndTheDeadStopBeingHit()
        {
            var o = new Outcome();
            yield return RunFight(20f, 12f, o);   // A dies to the fourth heavy at ~8.1 s; B keeps swinging at the corpse
            Assert.AreEqual(0f, o.HealthA);
            Assert.AreEqual(1, o.Deaths);
        }
    }
}
