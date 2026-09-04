using BossFight.Core;
using UnityEngine;

// Author: James Prendergast
// You are welcome to modify this for testing

namespace BossFight.Input
{
    /// <summary>
    /// A dummy body that does nothing. Useful for testing input without a player body.
    /// </summary>

    public class DummyBody : MonoBehaviour
    {
        private IIntentSource m_intentSource;

        void Start()
        {
            m_intentSource = GetComponent<IIntentSource>();
        }


        // Update is called once per frame
        void Update()
        {
            Intent intent = m_intentSource.GetIntent();

            Move(intent.Move);

            //Debug testing
            if ((intent.Debug & 1 << 0) != 0) {
                transform.Rotate(Vector3.forward, 12.5f);
            }
            if ((intent.Debug & 1 << 2) != 0) {
                transform.Rotate(Vector3.forward, -12.5f);
            }

            if (intent.Roll) {
                transform.Rotate(Vector3.left, 25f);
            }
            
        }

        private void Move(Vector3 movement)
        {
            transform.position += 5 * movement * Time.deltaTime;
        }
    }
}
