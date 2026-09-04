using UnityEngine;

namespace BossFight.Core
{
    /// <summary>
    /// One frame of "what the controller wants the body to do".
    /// Produced by a human input script, a scripted bot, or an ML agent.
    /// Consumed by the player body or the boss body; a body never cares who produced it.
    /// The field names match the Input Actions map on purpose.
    /// </summary>
    public struct Intent
    {
        /// <summary>World-space XZ direction to move, magnitude 0..1. Never raw stick input:
        /// the human controller converts stick to world direction using its camera, so bodies never need a camera.</summary>
        public Vector3 Move;

        /// <summary>Pressed this frame (edge-triggered).</summary>
        public bool LightAttack;

        /// <summary>Pressed this frame (edge-triggered).</summary>
        public bool HeavyAttack;

        /// <summary>Pressed this frame (edge-triggered).</summary>
        public bool Roll;


        /// <summary> a collection of bits determining which debug buttons have been requested. 4 bits long </summary>
        public int Debug;

        public static Intent None => default;
    }
}
