using UnityEngine;

namespace BossFight.Combat
{
    /// <summary>
    /// Marks a collider as "hitting this hurts the owner". Put it on the body's physical collider
    /// (or any child collider) and it routes hits to the <see cref="Health"/> on the body root.
    /// The collider must be on the body's layer (Player or Boss) so the collision matrix works.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Hurtbox : MonoBehaviour
    {
        [SerializeField] Health owner;

        public Health Owner => owner;

        void Awake()
        {
            if (owner == null) owner = GetComponentInParent<Health>();
        }

        void Reset() => owner = GetComponentInParent<Health>();
    }
}
