using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.Sensors
{
	[AddComponentMenu("Sensor/Markup Sensor")]
	public class MarkupSensor : BaseSensor, IPulseableSensor
	{
		#region Fields

		[SerializeField]
		protected SensorPulse m_pulse;

		[Min(0f)]
		public float radius = 5f;

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
		}

		protected void CustomPulse()
		{
			foreach (var markup in SensorManager.Instance.Get(gameObject, radius))
			{
				var detected = GetFilteredObject(markup.gameObject);
				if (detected == null)
					continue;

				m_pulse.AddPendingSignal(this, detected);
			}
		}

		protected override void CustomAddSignal(SensorEventArgs args)
		{
			var markup = args.signal.detected.GetComponent<Markup>();
			if (markup != null)
			{
				markup.onSignalDetected.Invoke(args);
			}
		}

		protected override void CustomRemoveSignal(SensorEventArgs args)
		{
			var obj = args.signal.detected;
			if (GameObjectExt.IsNull(obj))
				return;

			var markup = obj.GetComponent<Markup>();
			if (markup != null)
			{
				markup.onSignalUndetected.Invoke(args);
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
