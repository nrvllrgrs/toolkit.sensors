using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.Sensors
{
    public class SensorEventArgs
    {
        public BaseSensor sensor;
        public Signal signal;

		#region Methods

		public bool Equals(SensorEventArgs other)
		{
			if (ReferenceEquals(null, other))
				return false;

			if (ReferenceEquals(this, other))
				return true;

			return Equals(sensor, other.sensor)
                && Equals(signal, other.signal);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			if (ReferenceEquals(this, obj))
				return true;

			if (obj.GetType() != GetType())
				return false;

            return Equals((SensorEventArgs)obj);
		}

		public override int GetHashCode()
        {
            return HashCode.Combine(sensor?.GetHashCode() ?? 0, signal?.GetHashCode() ?? 0);
        }

        #endregion
    }

	public abstract class BaseSensor : MonoBehaviour, ISignalDetectable
    {
        #region Fields

        [SerializeField, Tooltip("Evaluators to calculate the strength of a signal.")]
        protected UnityEvaluator m_strengthCalculator = new();

        [SerializeField, Tooltip("Evaluators to filter objects.")]
        protected UnityEvaluator m_filters = new();

        [SerializeField]
        private Set m_set;

        protected Dictionary<GameObject, Signal> m_signals = new();

        #endregion

        #region Events

        [SerializeField, Tooltip("Invoked when a signal is detected AND no other signal was detected.")]
        private UnityEvent<SensorEventArgs> m_onFirstDetection = new UnityEvent<SensorEventArgs>();

        [SerializeField, Tooltip("Invoked when a signal is detected.")]
        private UnityEvent<SensorEventArgs> m_onSignalDetected = new UnityEvent<SensorEventArgs>();

        [SerializeField, Tooltip("Invoked when a signal is undetected.")]
        private UnityEvent<SensorEventArgs> m_onSignalUndetected = new UnityEvent<SensorEventArgs>();

        [SerializeField, Tooltip("Invoked when a signal is undetected AND no other signal is detected.")]
        private UnityEvent<SensorEventArgs> m_onLastUndetection = new UnityEvent<SensorEventArgs>();

        #endregion

        #region Properties

        public UnityEvaluator strengthCalculator => m_strengthCalculator;
        public UnityEvaluator filters => m_filters;
        public Set set => m_set;
        public GameObject[] keys => m_signals.Keys.ToArray();

        public Signal[] signals => m_signals.Values.ToArray();

        public Signal closestSignal
        {
            get
            {
                Signal closestSignal = null;
                float closestSqrDistance = float.MaxValue;

                var list = m_signals.Values.ToArray();
                foreach (var signal in list)
                {
                    if (signal.detected.IsNull())
                    {
                        m_signals.Remove(signal.detected);
                        continue;
                    }

                    float sqrDistance = (signal.position - transform.position).sqrMagnitude;
                    if (closestSignal == null || sqrDistance < closestSqrDistance)
                    {
                        closestSignal = signal;
                        closestSqrDistance = sqrDistance;
                    }
                }
                return closestSignal;
            }
        }

        public Signal strongestSignal
        {
            get
            {
                Signal strongestSignal = null;

				var list = m_signals.Values.ToArray();
				foreach (var signal in list)
                {
					if (signal.detected.IsNull())
					{
						m_signals.Remove(signal.detected);
						continue;
					}

					if (strongestSignal == null || signal.strength > strongestSignal.strength)
                    {
                        strongestSignal = signal;
                    }
                }
                return strongestSignal;
            }
        }

        public bool anySignal => m_signals.Count > 0;
        public bool anySignalWithStrength => m_signals.Any(x => x.Value.strength > 0);

        public UnityEvent<SensorEventArgs> onFirstDetection => m_onFirstDetection;
        public UnityEvent<SensorEventArgs> onSignalDetected => m_onSignalDetected;
        public UnityEvent<SensorEventArgs> onSignalUndetected => m_onSignalUndetected;
        public UnityEvent<SensorEventArgs> onLastUndetection => m_onLastUndetection;

		#endregion

		#region Methods

		public void AddSignal(GameObject detected, SignalType signalType = null)
        {
            AddSignal(detected, strengthCalculator?.Evaluate(gameObject, detected, detected.transform.position) ?? 1f, signalType);
        }

        public void AddSignal(GameObject detected, float strength, SignalType signalType = null)
        {
            if (!TryAddSignalToMap(detected, strength, signalType, m_signals, out Signal signal))
                return;

            FinalizeAddSignal(signal);
        }

        public void AddSignal(Signal signal)
        {
            if (signal == null)
                return;

            // Signal already exists, update strength and skip
            if (m_signals.TryGetValue(signal.detected, out Signal value))
            {
                value.strength = signal.strength;
                return;
            }

            m_signals.Add(signal.detected, signal);
            m_set?.Add(signal.detected);

            FinalizeAddSignal(signal);
        }

        private void FinalizeAddSignal(Signal signal)
        {
            var args = new SensorEventArgs()
            {
                sensor = this,
                signal = signal,
            };

            // Check if first signal
            if (m_signals.Count == 1)
            {
                onFirstDetection.Invoke(args);
            }
            onSignalDetected.Invoke(args);

            CustomAddSignal(args);
		}

        protected virtual void CustomAddSignal(SensorEventArgs args)
        { }

        public bool TryAddSignalToMap(GameObject detected, float strength, SignalType signalType, Dictionary<GameObject, Signal> map, out Signal signal)
        {
            // Signal already exists, skip
            if (map.TryGetValue(detected, out signal))
            {
                signal.strength = strength;
                return false;
            }

            // Create new signal
            signal = new Signal(detected, strength, signalType);

            map.Add(detected, signal);
            return true;
        }

        public virtual void RemoveSignal(GameObject detected)
        {
            // Signal does not exists, skip
            if (!m_signals.TryGetValue(detected, out Signal signal))
                return;

            var args = new SensorEventArgs()
            {
                sensor = this,
                signal = signal,
            };

            m_signals.Remove(detected);
            m_set?.Remove(signal.detected);

            onSignalUndetected.Invoke(args);

            // Not detecting any signals
            if (m_signals.Count == 0)
            {
                onLastUndetection.Invoke(args);
            }

            CustomRemoveSignal(args);
		}

        protected virtual void CustomRemoveSignal(SensorEventArgs args)
        { }

        public void ClearSignals()
        {
            var targets = m_signals.Keys.ToArray();
            foreach (var target in targets)
            {
                RemoveSignal(target);
            }
        }

        public void CleanSignals()
        {
            var targets = m_signals.Keys.ToArray();
            foreach (var target in targets)
            {
                if (target.IsNull())
                {
                    RemoveSignal(target);
                }
            }
        }

        public bool TryGetClosetTarget(out Vector3 target)
        {
            return TryGetSignalTarget(closestSignal, out target);
        }

        public bool TryGetStrongestTarget(out Vector3 target)
        {
            return TryGetSignalTarget(strongestSignal, out target);
        }

        public bool TryGetSignalTarget(Signal signal, out Vector3 target)
        {
            if (signal == null)
            {
                target = default;
                return false;
            }

            var losTarget = signal.detected.GetComponent<LOSTargets>();
            if (losTarget == null)
            {
                target = signal.position;
            }
            else
            {
                target = losTarget.Targets[0].Point.position;
            }
            return true;
        }

        public bool IsDetecting(GameObject obj, bool includeChildren = false)
        {
            if (obj == null)
                return false;

			if (!includeChildren)
				return m_signals.ContainsKey(obj);

            foreach (var signalKey in m_signals.Keys)
            {
				Transform parent = signalKey.transform;
				while (parent != null)
				{
					if (ReferenceEquals(obj, parent.gameObject))
						return true;

					parent = parent.parent;
				}
			}
            return false;
		}

        protected virtual bool IsIncluded(GameObject detected)
        {
            if (detected == null)
                return false;

            return m_filters.Evaluate(gameObject, detected, detected.transform.position) != 0f;
        }

        protected GameObject GetFilteredObject(GameObject detected) => IsIncluded(detected) ? detected : null;

        #endregion
    }
}
