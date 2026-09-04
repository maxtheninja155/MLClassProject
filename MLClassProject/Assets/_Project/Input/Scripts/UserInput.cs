using BossFight.Core;
using UnityEngine;
using UnityEngine.InputSystem;

// Author: James Prendergast

namespace BossFight.Input
{
    /// <summary>
    /// Reads input from the user and formats it into an Intent to be sent to a body
    /// Also resolve camera guidance through Cinemachine
    /// </summary>

    public class UserInput : MonoBehaviour, IIntentSource
    {
        [SerializeField] private Camera m_camera;

        #region Queued Input

        private Vector3 m_queuedMove = Vector3.zero;
        private bool m_queuedRoll = false;
        private bool m_queuedLightAttack = false;
        private bool m_queuedHeavyAttack = false;

        private Intent m_intent;

        #endregion

        /// <summary>
        /// Reads and caches the inputed movement value
        /// contextualizes raw input into world space
        /// </summary>
        private void LateUpdate() {
            // compiles intent
            m_intent = new Intent {
                Move = m_queuedMove,
                Roll = m_queuedRoll,
                LightAttack = m_queuedLightAttack,
                HeavyAttack = m_queuedHeavyAttack
            };
        }

        /// <summary>
        /// Interface override
        /// Returns the current stored intent
        /// At least one frame behind
        /// </summary>
        /// <returns></returns>
        public Intent GetIntent() {
            return m_intent;
        }

        #region Input Reading

        /// <summary>
        /// Reads and caches the inputed movement value
        /// contextualizes raw input into world space
        /// </summary>
        /// <param name="value"></param>
        public void OnMove(InputValue value) {
            Vector2 inputVector = value.Get<Vector2>();

            // queue input relative to camera orientation on XZ plane
            Vector3 facing = Vector3.ProjectOnPlane(m_camera.transform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(m_camera.transform.right, Vector3.up).normalized;
            m_queuedMove = facing * inputVector.y + right * inputVector.x;
        }

        /// <summary>
        /// queues a roll input for the next frame
        /// </summary>
        /// <param name="value"></param>
        public void OnRoll(InputValue value) {
            m_queuedRoll = value.isPressed;
        }

        /// <summary>
        /// queues a light attack input for the next frame
        /// </summary>
        /// <param name="value"></param>
        public void OnLightAttack(InputValue value) {
            m_queuedLightAttack = value.isPressed;
        }

        /// <summary>
        /// queues a heavy attack input for the next frame
        /// </summary>
        /// <param name="value"></param>
        public void OnHeavyAttack(InputValue value) {
            m_queuedHeavyAttack = value.isPressed;
        }

        #endregion
    }
}
