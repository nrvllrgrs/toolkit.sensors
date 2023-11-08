using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.Sensors
{
    public enum PulseMode
    {
        Manual,
        EveryFrame,
        FixedInterval,
    }

    [System.Serializable]
    public class SensorPulse
    {
        #region Fields

        public PulseMode pulseMode;

        private Dictionary<GameObject, Signal> m_pendingSignals = new();

        #endregion

        #region Events

        [Tooltip("Invoked when pulsed.")]
        public UnityEvent<SensorEventArgs> OnPulsed = new UnityEvent<SensorEventArgs>();

        #endregion

        #region Properties

        public System.Action CustomPulse = null;

        #endregion

        #region Methods

        public void Pulse(BaseSensor sensor)
        {
            // Collect pending signals
            m_pendingSignals.Clear();
            CustomPulse?.Invoke();

            // Remove signals not detected during this pulse
            var signalKeysToRemove = sensor.keys.Except(m_pendingSignals.Keys).ToArray();
            foreach (var key in signalKeysToRemove)
            {
                sensor.RemoveSignal(key);
            }

            // Add pending signals
            foreach (var pendingSignal in m_pendingSignals.Values)
            {
                sensor.AddSignal(pendingSignal);
            }
        }

        public void AddPendingSignal(BaseSensor sensor, GameObject detected, SignalType signalType = null)
        {
            AddPendingSignal(sensor, detected, sensor.strengthCalculator?.Evaluate(sensor.gameObject, detected) ?? 1f, signalType);
        }

        public void AddPendingSignal(BaseSensor sensor, GameObject detected, Vector3 position, SignalType signalType = null)
        {
            AddPendingSignal(sensor, detected, sensor.strengthCalculator?.Evaluate(sensor.gameObject, detected, position) ?? 1f, signalType);
        }

        public void AddPendingSignal(BaseSensor sensor, GameObject detected, float strength, SignalType signalType = null)
        {
            sensor.TryAddSignalToMap(detected, strength, signalType, m_pendingSignals, out Signal signal);
        }

        #endregion
    }
}