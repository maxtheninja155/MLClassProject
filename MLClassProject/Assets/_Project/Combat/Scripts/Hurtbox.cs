using BossFight.Core;
using UnityEngine;

namespace BossFight.Combat
{
    /// <summary>
    /// Marks a collider as "hitting this hurts the owner". Put it on the body's physical collider or any child collider;
    /// colliders below it count too. Hits route to the nearest <see cref="IDamageable"/> up the hierarchy
    /// (normally <see cref="Health"/> on the body root). The collider must be on the body's layer (Player or Boss)
    /// so the collision matrix works.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Hurtbox : MonoBehaviour
    {
        IDamageable owner;

        public IDamageable Owner => owner ??= GetComponentInParent<IDamageable>();
    }
}
