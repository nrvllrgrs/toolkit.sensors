using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace ToolkitEngine.Sensors
{
    [AddComponentMenu("Sensor/Filter Sensor")]
    public class FilterSensor : BaseSensor, IPulseableSensor
    {
        #region Fields

        [SerializeField, Required]
        private BaseSensor m_sensor;

        [SerializeField, Tooltip("Indicates whether referenced sensor should automatically pulse prior to pulsing.")]
        private bool m_autoPulse;

        [SerializeField]
        private SensorPulse m_pulse;

        [SerializeField]
        private UnityEvent<SensorEventArgs> m_onPulsed;

        private IPulseableSensor m_pulseableSensor;

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
            Assert.IsNotNull(m_sensor, "Sensor is undefined!");
            m_sensor.onFirstDetection.AddListener(Sensor_Detection);
            m_sensor.onLastUndetection.AddListener(Sensor_NoDetection);

            m_pulse.CustomPulse = CustomPulse;
            m_pulseableSensor = m_sensor as IPulseableSensor;
		}

        private void OnDestroy()
        {
            if (m_sensor != null)
            {
                m_sensor.onFirstDetection.RemoveListener(Sensor_Detection);
                m_sensor.onLastUndetection.RemoveListener(Sensor_NoDetection);
            }
        }

        protected override bool IsIncluded(GameObject detected)
        {
			if (detected == null)
				return false;

			return m_filters.Evaluate(m_sensor.gameObject, detected, detected.transform.position) != 0f;
        }

        private void Sensor_Detection(SensorEventArgs e)
        {
            // Wait until something is detected
            // No need to pulse if checking against an empty set
            SensorManager.Instance.Register(this);
        }

        private void Sensor_NoDetection(SensorEventArgs e)
        {
            if (SensorManager.Exists)
            {
                SensorManager.Instance.Unregister(this);
                ClearSignals();
            }
        }

        public void Pulse()
        {
            if (m_autoPulse && m_pulseableSensor != null)
            {
				m_pulseableSensor.Pulse();
			}

            m_pulse.Pulse(this);
            m_onPulsed?.Invoke(new SensorEventArgs()
            {
                sensor = this,
            });
        }

        protected void CustomPulse()
        {
            foreach (var signal in m_sensor.signals)
            {
                var detected = GetFilteredObject(signal.detected);
                if (detected == null)
                    continue;

                m_pulse.AddPendingSignal(this, detected);
            }
        }

        #endregion
    }
}