using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.Sensors
{
    public class MarkupEventArgs : System.EventArgs
    {
		#region Properties

        public Markup markup { get; private set; }
        public GameObject actor { get; private set; }

		#endregion

		#region Constructors

        public MarkupEventArgs(Markup markup, GameObject actor)
        {
            this.markup = markup;
            this.actor = actor;
        }

		#endregion
	}

	public class Markup : MonoBehaviour, ISignalDetectable
    {
        #region Fields

        [SerializeField]
        private MarkupType m_type;

        [SerializeField, Min(0f)]
        private float m_radius = 0f;

		[SerializeField, Min(0f)]
		private float m_height = 0f;

        private HashSet<BaseSensor> m_detectedBy = new();

		#endregion

		#region Events

		[SerializeField]
		private UnityEvent<SensorEventArgs> m_onFirstDetection;

		[SerializeField]
        private UnityEvent<SensorEventArgs> m_onSignalDetected;

		[SerializeField]
		private UnityEvent<SensorEventArgs> m_onSignalUndetected;

		[SerializeField]
		private UnityEvent<SensorEventArgs> m_onLastUndetection;

		#endregion

		#region Properties

		public MarkupType type => m_type;

        public float radius => m_radius;
        public float height => m_height;

        /// <summary>
        /// Indicates whether Markup is detected by any MarkupSensor
        /// </summary>
		public bool isDetected => m_detectedBy.Count > 0;

		public UnityEvent<SensorEventArgs> onFirstDetection => m_onFirstDetection;

        /// <summary>
        /// Invoked when this Markup is detected by any MarkupSensor
        /// </summary>
        public UnityEvent<SensorEventArgs> onSignalDetected => m_onSignalDetected;

        /// <summary>
        /// Invoked when this Markup is undetected by any MarkupSensor
        /// </summary>
        public UnityEvent<SensorEventArgs> onSignalUndetected => m_onSignalUndetected;

        public UnityEvent<SensorEventArgs> onLastUndetection => m_onLastUndetection;

		#endregion

		#region Methods

		private void OnEnable()
        {
            SensorManager.CastInstance.Register(this);
            m_onSignalDetected.AddListener(SignalDetected);
            m_onSignalUndetected.AddListener(SignalUndetected);
        }

        private void OnDisable()
        {
			m_onSignalDetected.RemoveListener(SignalDetected);
			m_onSignalUndetected.RemoveListener(SignalUndetected);
			SensorManager.CastInstance.Unregister(this);
		}

		#endregion

		#region Detection Methods

        private void SignalDetected(SensorEventArgs e)
        {
            if (m_detectedBy.Count == 0)
            {
                m_onFirstDetection?.Invoke(e);
            }
            m_detectedBy.Add(e.sensor);
        }

        private void SignalUndetected(SensorEventArgs e)
        {
            m_detectedBy.Remove(e.sensor);
            if (m_detectedBy.Count == 0)
            {
                m_onLastUndetection?.Invoke(e);
            }
        }

        public bool IsDetectedBy(BaseSensor sensor) => m_detectedBy.Contains(sensor);

		#endregion

		#region Editor-Only
#if UNITY_EDITOR

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;

			if (m_radius > 0f)
            {
				if (m_height == 0f)
				{
					Gizmos.DrawWireSphere(transform.position, m_radius);
				}
                else
                {
					GizmosUtil.DrawCylinder(transform.position, m_radius, m_height, Color.blue);
				}
			}
            else if (m_height > 0f)
            {
                var offset = Vector3.up * (m_height * 0.5f);
				Gizmos.DrawLine(transform.position - offset, transform.position + offset);
			}
		}

#endif
		#endregion
	}
}