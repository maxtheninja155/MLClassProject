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
        public Vector2 Move;

        /// <summary>Raw look input (mouse delta / right stick). Only a human controller's camera reads this; bodies ignore it.</summary>
        public Vector2 Look;

        /// <summary>Pressed this frame (edge-triggered).</summary>
        public bool LightAttack;

        /// <summary>Pressed this frame (edge-triggered).</summary>
        public bool HeavyAttack;

        /// <summary>Pressed this frame (edge-triggered).</summary>
        public bool Roll;

        /// <summary>Pressed this frame (edge-triggered). The body treats it as a toggle.</summary>
        public bool LockOn;

        /// <summary>Held.</summary>
        public bool Sprint;

        public static Intent None => default;
    }
}
