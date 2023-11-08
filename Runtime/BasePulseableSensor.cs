using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.Sensors
{
    public abstract class BasePulseableSensor : BaseDetectionSensor, IPulseableSensor
    {
        #region Fields

        [SerializeField]
        protected SensorPulse m_pulse;

        [SerializeField, Tooltip("Invoked when pulsed.")]
        private UnityEvent<SensorEventArgs> m_onPulsed;

        #endregion

        #region Events

        public UnityEvent<SensorEventArgs> onPulsed => m_onPulsed;

        #endregion

        #region Properties

        public PulseMode pulseMode => m_pulse.pulseMode;

        #endregion

        #region Methods

        private void Awake()
        {
            m_pulse.CustomPulse = CustomPulse;
        }

        private void OnEnable()
        {
            SensorManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            ClearSignals();

            if (SensorManager.Exists)
            {
                SensorManager.Instance.Unregister(this);
            }
        }

        public void Pulse()
        {
            m_pulse.Pulse(this);
            m_onPulsed?.Invoke(new SensorEventArgs()
            {
                sensor = this,
            });
        }

        protected abstract void CustomPulse();

        #endregion
    }
}