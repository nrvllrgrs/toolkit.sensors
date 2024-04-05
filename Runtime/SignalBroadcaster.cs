using UnityEngine;
using NaughtyAttributes;

namespace ToolkitEngine.Sensors
{
    [AddComponentMenu("Sensor/Signal Broadcaster")]
    public class SignalBroadcaster : MonoBehaviour
    {
        #region Fields

        [SerializeField, Tooltip("Type of signal to be broadcast.")]
        private SignalType m_signal;

        [SerializeField, Tooltip("Strength factor of broadcast signal.")]
        private float m_factor = 1f;

        [SerializeField, Label("Frequency")]
        private PulseMode m_pulseMode;

        #endregion

        #region Properties

        public PulseMode PulseMode => m_pulseMode;

        #endregion

        #region Methods

        private void OnEnable()
        {
            SensorManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            if (SensorManager.Exists)
            {
                SensorManager.Instance.Unregister(this);
            }
        }

        [ContextMenu("Broadcast")]
        public void Broadcast()
        {
            Signal.Broadcast(gameObject, transform.position, m_factor, m_signal);
        }

        #endregion
    }
}