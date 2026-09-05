using BossFight.Core;
using UnityEngine;

namespace BossFight.Combat.Sandbox
{
    /// <summary>Sandbox only: prints every hit and death to the console so you can see the events fire.</summary>
    public class FightEventLogger : MonoBehaviour
    {
        void OnEnable()
        {
            FightEvents.OnHit += LogHit;
            FightEvents.OnDeath += LogDeath;
        }

        void OnDisable()
        {
            FightEvents.OnHit -= LogHit;
            FightEvents.OnDeath -= LogDeath;
        }

        static void LogHit(GameObject attacker, GameObject victim, DamageInfo info)
        {
            var health = victim.GetComponent<Health>();
            Debug.Log($"[Fight] {attacker?.name ?? "?"} hit {victim.name} for {info.Amount} ({health?.Current}/{health?.Max} left)");
        }

        static void LogDeath(GameObject victim) => Debug.Log($"[Fight] {victim.name} died");
    }
}
