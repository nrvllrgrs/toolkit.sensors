using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToolkitEngine.Sensors
{
    [AddComponentMenu("Sensor/Signal Sensor")]
    public class SignalSensor : BaseSensor
    {
        #region Enumerators

        public enum ForgetMode
        {
            Manual,
            Time,
            Rate,
        }

        #endregion

        #region Fields

        [Tooltip("Indiates whether received signal's strength is immediately calculated and added to signals.")]
        public bool immediate;

        [Min(0f)]
        public float radius = 5f;

        [Tooltip("List of signals that can be detected by this sensor. If empty, this sensor will detect ALL signals.")]
        public List<SignalType> validSignals;

        public ForgetMode forgetMode;

        [Min(0f), Tooltip("Seconds to forget signal.")]
        public float forgetTime = 0.02f;

        [Min(0f), Tooltip("Strength lost per second.")]
        public float forgetRate = 0.2f;

        private Dictionary<GameObject, float> m_detectedLifetimes = new();
        private Dictionary<Signal, float> m_pendingSignalTimestamps = new();
        private const float PENDING_FORGET_TIME = 0.02f;

        #endregion

        #region Methods

        private void OnEnable()
        {
            Signal.Broadcasted += Signal_Broadcasted;
        }

        private void OnDisable()
        {
            Signal.Broadcasted -= Signal_Broadcasted;
        }

        private void Signal_Broadcasted(object sender, SensorEventArgs e)
        {
            // Signal is outside of range, skip
            if ((e.signal.position - transform.position).sqrMagnitude > radius * radius)
                return;

            // Signal is not a valid type, skip
            if (e.signal.type != null && validSignals.Count > 0 && !validSignals.Any(x => x.IsSupertypeOf(e.signal.type)))
                return;

            if (immediate)
            {
                // Add lifetime BEFORE adding signal so can easily check if first detection
                RestartLifetime(e.signal.detected);

                // Calculate signal strength using received strength as factor
                AddSignal(e.signal.detected, e.signal.strength * strengthCalculator?.Evaluate(gameObject, e.signal.detected, e.signal.position) ?? 1f);
            }
            else
            {
                // Need to make a new signal for this sensor
                // Other sensors are receiving the same signal
                var signal = new Signal(e.signal);

                if (!m_pendingSignalTimestamps.ContainsKey(signal))
                {
                    m_pendingSignalTimestamps.Add(signal, Time.time);
                }
                else
                {
                    m_pendingSignalTimestamps[signal] = Time.time;
                }
            }
        }

        public bool ProcessPending()
        {
            foreach (var p in m_pendingSignalTimestamps)
            {
                if (Time.time > p.Value + PENDING_FORGET_TIME)
                    continue;

                // Calculate signal strength using received strength as factor
                p.Key.strength *= strengthCalculator?.Evaluate(gameObject, p.Key.detected, p.Key.detected.transform.position) ?? 1f;

                // Add lifetime BEFORE adding signal so can easily check if first detection
                RestartLifetime(p.Key.detected);
                AddSignal(p.Key);
            }

            m_pendingSignalTimestamps.Clear();
            return anySignal;
        }

        public void Tick()
        {
            GameObject[] keys;
            switch (forgetMode)
            {
                case ForgetMode.Time:
                    keys = m_detectedLifetimes.Keys.ToArray();
                    foreach (var key in keys)
                    {
                        m_detectedLifetimes[key] += Time.deltaTime;
                        if (m_detectedLifetimes[key] >= forgetTime)
                        {
                            RemoveSignal(key);
                        }
                    }
                    break;

                case ForgetMode.Rate:
                    keys = base.keys;
                    foreach (var key in keys)
                    {
                        m_signals[key].strength -= forgetRate * Time.deltaTime;
                        if (m_signals[key].strength <= 0)
                        {
                            RemoveSignal(key);
                        }
                    }
                    break;
            }

            if (!anySignal && SensorManager.Exists)
            {
                SensorManager.Instance.Unregister(this);
            }
        }

        private void RestartLifetime(GameObject detected)
        {
            if (!anySignal)
            {
                SensorManager.Instance.Register(this);
            }

            if (!m_detectedLifetimes.ContainsKey(detected))
            {
                m_detectedLifetimes.Add(detected, 0f);
            }
            else
            {
                m_detectedLifetimes[detected] = 0f;
            }
        }

        #endregion

        #region Editor-Only
#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

#endif
        #endregion
    }
}