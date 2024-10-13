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

		[SerializeField, Min(0f)]
		private float m_radius = 5f;

		[SerializeField, Min(0f)]
		private float m_height;

		[SerializeField, Tooltip("Invoked when pulsed.")]
		private UnityEvent<SensorEventArgs> m_onPulsed;

		#endregion

		#region Events

		public UnityEvent<SensorEventArgs> onPulsed => m_onPulsed;

		#endregion

		#region Properties

		public PulseMode pulseMode => m_pulse.pulseMode;
		public float radius => m_radius;
		public float height => m_height;

		#endregion

		#region Methods

		private void Awake()
		{
			m_pulse.CustomPulse = CustomPulse;
		}

		private void OnEnable()
		{
			SensorManager.CastInstance.Register(this);
		}

		private void OnDisable()
		{
			ClearSignals();
			SensorManager.CastInstance.Unregister(this);
		}

		public void Pulse()
		{
			m_pulse.Pulse(this);
			m_onPulsed?.Invoke(new SensorEventArgs()
			{
				sensor = this,
			});
		}

		protected void CustomPulse()
		{
			foreach (var markup in SensorManager.CastInstance.Get(gameObject, m_radius, m_height))
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
			if (m_radius == 0)
				return;

			if (m_height == 0f)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(transform.position, m_radius);
			}
			else if (m_radius > 0f)
			{
				GizmosUtil.DrawCylinder(transform.position, m_radius, m_height, Color.green);
			}
		}

#endif
		#endregion
	}
}
