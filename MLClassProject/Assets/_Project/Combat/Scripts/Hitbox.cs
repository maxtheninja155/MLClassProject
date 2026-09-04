using System.Collections.Generic;
using BossFight.Core;
using UnityEngine;

namespace BossFight.Combat
{
    /// <summary>
    /// A sphere of damage. <see cref="AttackRunner"/> arms it for the attack's active window and disarms it after.
    /// While armed it runs one overlap query per physics step and hits each target at most once per swing.
    /// Needs no collider or rigidbody. Put it on a child of the body (one per weapon or fist, more if a move
    /// needs coverage) on the PlayerHitbox or BossHitbox layer: the layer collision matrix decides what it can hit.
    /// </summary>
    public class Hitbox : MonoBehaviour
    {
        [Tooltip("Sphere center in this object's local space.")]
        [SerializeField] Vector3 offset = Vector3.zero;
        [SerializeField, Min(0.01f)] float radius = 0.5f;

        GameObject owner;
        AttackData attack;
        int layerMask;
        readonly HashSet<Health> hitThisSwing = new HashSet<Health>();
        readonly Collider[] buffer = new Collider[32];

        public bool IsArmed { get; private set; }

        void Awake() => layerMask = MaskFromCollisionMatrix(gameObject.layer);

        /// <summary>Everything the collision matrix lets this layer touch.</summary>
        static int MaskFromCollisionMatrix(int layer)
        {
            int mask = 0;
            for (int i = 0; i < 32; i++)
                if (!Physics.GetIgnoreLayerCollision(layer, i)) mask |= 1 << i;
            return mask;
        }

        public void Arm(AttackData data, GameObject attacker)
        {
            attack = data;
            owner = attacker;
            hitThisSwing.Clear();
            IsArmed = true;
        }

        public void Disarm()
        {
            IsArmed = false;
            attack = null;
        }

        void FixedUpdate()
        {
            if (!IsArmed || attack == null) return;
            int count = Overlap();
            for (int i = 0; i < count; i++) TryHit(buffer[i]);
        }

        int Overlap()
        {
            var center = transform.TransformPoint(offset);
            var s = transform.lossyScale;
            float worldRadius = radius * Mathf.Max(s.x, s.y, s.z);
            return Physics.OverlapSphereNonAlloc(center, worldRadius, buffer, layerMask, QueryTriggerInteraction.Collide);
        }

        void TryHit(Collider other)
        {
            var hurt = other.GetComponentInParent<Hurtbox>();
            if (hurt == null || hurt.Owner == null) return;
            if (owner != null && hurt.Owner.transform.IsChildOf(owner.transform)) return;   // never hit yourself
            if (!hitThisSwing.Add(hurt.Owner)) return;

            hurt.Owner.TakeDamage(new DamageInfo(attack.Damage, owner));
        }

        void OnDrawGizmos()
        {
            Gizmos.color = IsArmed ? new Color(1f, 0.2f, 0.1f, 0.6f) : new Color(1f, 0.6f, 0.1f, 0.15f);
            var s = transform.lossyScale;
            Gizmos.DrawSphere(transform.TransformPoint(offset), radius * Mathf.Max(s.x, s.y, s.z));
        }
    }
}
