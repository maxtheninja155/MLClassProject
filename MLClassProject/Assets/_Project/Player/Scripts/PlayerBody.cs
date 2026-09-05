using System.Collections.Generic;
using BossFight.Core;
using UnityEngine;

// Author: James Prendergast

namespace BossFight.Player
{
    public class PlayerBody : MonoBehaviour
    {
        #region Player Params

        // amount of time left until action ready, see ActionReady()
        // set when an action is taken depending on its duration
        // also to be set when taking damage or other interventions
        private float m_cooldownTimer = 0f;

        // state based controller
        // Movement is handled independently of the buffer system
        // serves as system state definition
        private enum Action {
            None,
            Roll,
            LightAttack,
            HeavyAttack
        }
        private Action m_currentAction = Action.None;

        // Base movement speed walking around
        [SerializeField] private float m_speed = 5f;

        [Header("Roll")]
        [SerializeField] private float m_rollDistance = 3f;
        [SerializeField] private float m_rollDuration = 1f;

        [Header("Light Attack")]
        // TODO: add damage or otherwise
        [SerializeField] private float m_lightAttackDuration = .5f;

        [Header("Heavy Attack")]
        // TODO: add damage or otherwise
        [SerializeField] private float m_heavyAttackDuration = 1.5f;

        #endregion

        /// <summary>
        /// Gets reference to intent source
        /// </summary>
        void Start() {
            m_intentSource = GetComponent<IIntentSource>();
        }

        /// <summary>
        /// Calls corresponding handlers
        /// </summary>
        void Update() {
            // reduce cooldown timer
            if (m_cooldownTimer > 0f) {
                m_cooldownTimer -= Time.deltaTime;
            }

            // Process this frame's intent and resolve buffer
            ProcessIntent();
        }

        #region Input/Intent

        // if input is recieved while action is unavailable
        // it will be stored in a buffer for this many seconds before being discarded
        [SerializeField] private float m_inputBuffer = .5f;
        private IIntentSource m_intentSource;

        // Requested input is processed through a buffer
        struct bufferedInput {
            public Action action;
            public float timeOfRequest;
        }
        private List<bufferedInput> m_bufferedInputs = new List<bufferedInput>();

        /// <summary>
        /// Called in update loop
        /// Reads intent into buffered input
        /// enacts upon (or cleans) buffered input if possible
        /// </summary>
        private void ProcessIntent() {
            // read this frame's intent
            Intent intent = m_intentSource.GetIntent();

            // resolves action request in the order of events decided below
            if (intent.Roll)
                BufferInput(Action.Roll);
            if (intent.LightAttack)
                BufferInput(Action.LightAttack);
            if (intent.HeavyAttack)
                BufferInput(Action.HeavyAttack);
        
            // process buffered input
            for (int i = 0; i < m_bufferedInputs.Count; i++) {
                bufferedInput input = m_bufferedInputs[i];

                // discard expired input
                if (Time.time - input.timeOfRequest > m_inputBuffer) {
                    m_bufferedInputs.RemoveAt(i);
                    i--;
                    continue;
                }
                
                // try to resolve valid buffered input
                if (ActionReady()) {
                    // enact valid input
                    switch (input.action) {
                        case Action.Roll:
                            Roll();
                            break;
                        case Action.LightAttack:
                            LightAttack();
                            break;
                        case Action.HeavyAttack:
                            HeavyAttack();
                            break;
                    }
                    // remove this action from the buffer
                    m_bufferedInputs.RemoveAt(i);
                    i--;
                } else {
                    // no need to continue resolving buffered input if no action can be made
                    break;
                }
            }
        }

        /// <summary>
        /// buffers the desired action at the time of calling
        /// </summary>
        /// <param name="action">what action to buffer</param>
        private void BufferInput(Action action) {
            Debug.Log("Buffered action: " + action);
            m_bufferedInputs.Add(new bufferedInput { action = action, timeOfRequest = Time.time });
        }
        
        #endregion

        #region Actions

        /// <summary>
        /// Checks if the player is ready to perform an action
        /// </summary>
        /// <returns>if there is no action cooldown left</returns>
        private bool ActionReady() {
            return m_cooldownTimer <= 0f;
        }
        /// <summary>
        /// Rolls the player in the current direction of movement
        /// initiates an action cooldown
        /// </summary>
        private void Roll() {
            m_cooldownTimer = m_rollDuration;
            Debug.Log("Rolling");
        }

        /// <summary>
        /// Conducts a light attack
        /// initiates an action cooldown
        /// </summary>
        private void LightAttack() {
            m_cooldownTimer = m_lightAttackDuration;
            Debug.Log("Light Attacking");
        }

        /// <summary>
        /// Conducts a heavy attack
        /// initiates an action cooldown
        /// </summary>
        private void HeavyAttack() {
            m_cooldownTimer = m_heavyAttackDuration;
            Debug.Log("Heavy Attacking");
        }

        #endregion
    }
}
