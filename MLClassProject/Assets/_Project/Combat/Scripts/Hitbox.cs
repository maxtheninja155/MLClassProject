using System.Collections.Generic;
using BossFight.Core;
using UnityEngine;

namespace BossFight.Combat
{
    /// <summary>
    /// A damage volume. <see cref="AttackRunner"/> arms it for the attack's active window and disarms it after.
    /// While armed it runs an overlap query every physics step and hits each target at most once per swing.
    /// It does not use trigger events on purpose: a hitbox that never moves stops getting them once PhysX sleeps it.
    ///
    /// Put it on a child of the body with a Sphere, Box, or Capsule collider that describes the volume.
    /// The collider itself stays disabled; only its shape is read. Put the object on the PlayerHitbox or BossHitbox
    /// layer: the layer collision matrix decides what it can hit.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Hitbox : MonoBehaviour
    {
        Collider col;
        GameObject owner;
        AttackData attack;
        int layerMask;
        readonly HashSet<Health> hitThisSwing = new HashSet<Health>();
        readonly Collider[] buffer = new Collider[32];

        public bool IsArmed { get; private set; }

        void Awake()
        {
            col = GetComponent<Collider>();
            col.enabled = false;
            layerMask = MaskFromCollisionMatrix(gameObject.layer);
        }

        void Reset() => GetComponent<Collider>().isTrigger = true;

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
            var t = transform;
            var scale = t.lossyScale;
            switch (col)
            {
                case SphereCollider s:
                {
                    float r = s.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
                    return Physics.OverlapSphereNonAlloc(t.TransformPoint(s.center), r, buffer, layerMask, QueryTriggerInteraction.Collide);
                }
                case BoxCollider b:
                {
                    var half = Vector3.Scale(b.size, scale) * 0.5f;
                    half = new Vector3(Mathf.Abs(half.x), Mathf.Abs(half.y), Mathf.Abs(half.z));
                    return Physics.OverlapBoxNonAlloc(t.TransformPoint(b.center), half, buffer, t.rotation, layerMask, QueryTriggerInteraction.Collide);
                }
                case CapsuleCollider c:
                {
                    Vector3 axis = c.direction == 0 ? Vector3.right : c.direction == 1 ? Vector3.up : Vector3.forward;
                    float axisScale = c.direction == 0 ? scale.x : c.direction == 1 ? scale.y : scale.z;
                    float radiusScale = c.direction == 0 ? Mathf.Max(scale.y, scale.z) : c.direction == 1 ? Mathf.Max(scale.x, scale.z) : Mathf.Max(scale.x, scale.y);
                    float r = c.radius * Mathf.Abs(radiusScale);
                    float half = Mathf.Max(0f, c.height * 0.5f * Mathf.Abs(axisScale) - r);
                    var center = t.TransformPoint(c.center);
                    var dir = t.TransformDirection(axis).normalized;
                    return Physics.OverlapCapsuleNonAlloc(center + dir * half, center - dir * half, r, buffer, layerMask, QueryTriggerInteraction.Collide);
                }
                default:
                    Debug.LogWarning($"{name}: Hitbox supports Sphere, Box, and Capsule colliders only.", this);
                    return 0;
            }
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
            var c = col != null ? col : GetComponent<Collider>();
            if (c == null) return;
            Gizmos.color = IsArmed ? new Color(1f, 0.2f, 0.1f, 0.6f) : new Color(1f, 0.6f, 0.1f, 0.15f);
            Gizmos.matrix = transform.localToWorldMatrix;
            switch (c)
            {
                case SphereCollider s: Gizmos.DrawSphere(s.center, s.radius); break;
                case BoxCollider b: Gizmos.DrawCube(b.center, b.size); break;
                case CapsuleCollider cc: Gizmos.DrawWireSphere(cc.center, cc.radius); break;
            }
        }
    }
}
