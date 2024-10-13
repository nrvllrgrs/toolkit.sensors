using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

		private GameObject m_reserver;
		private GameObject m_occupant;

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

		[SerializeField]
        private UnityEvent<MarkupEventArgs> m_onArrival;

        [SerializeField]
        private UnityEvent<MarkupEventArgs> m_onDeparture;

		[SerializeField]
		private UnityEvent<MarkupEventArgs> m_onReserved;

		[SerializeField]
		private UnityEvent<MarkupEventArgs> m_onCanceled;

		#endregion

		#region Properties

		public MarkupType type => m_type;

        public float radius => m_radius;
        public float height => m_height;

        /// <summary>
        /// Indicates whether markup is currently occupied
        /// </summary>
        public bool vacant => m_occupant == null;

        /// <summary>
        /// Indicates whether markup is being reserved by actor
        /// </summary>
        public bool reserved => m_reserver != null;

        public GameObject occupant => m_occupant;
        public GameObject reserver => m_reserver;

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

        public UnityEvent<MarkupEventArgs> onArrival => m_onArrival;
        public UnityEvent<MarkupEventArgs> onDeparture => m_onDeparture;
        public UnityEvent<MarkupEventArgs> onReserved => m_onReserved;
        public UnityEvent<MarkupEventArgs> onCanceled => m_onCanceled;

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

        public virtual bool Reserve(GameObject obj)
        {
            // Already occupied, cannot reserve
            if (!vacant)
                return false;

            m_reserver = obj;
            m_onReserved?.Invoke(new MarkupEventArgs(this, obj));
            return true;
		}

        public virtual bool Cancel(GameObject obj)
        {
            if (!ReservedBy(obj))
                return false;

            ForceCancel();
			return true;
        }

        public void ForceCancel()
        {
            var t = m_reserver;
            m_reserver = null;

			m_onReserved?.Invoke(new MarkupEventArgs(this, t));
		}

        public virtual bool Arrive(GameObject obj)
        {
            if (!CanOccupy(obj))
                return false;

            m_reserver = null;
            m_occupant = obj;
			m_onArrival?.Invoke(new MarkupEventArgs(this, obj));
			return true;
        }

        public virtual bool Depart(GameObject obj)
        {
            if (!OccupiedBy(obj))
                return false;

            m_occupant = null;
			m_onDeparture?.Invoke(new MarkupEventArgs(this, obj));
			return true;
        }

        public virtual void Evict()
        {
            m_occupant = null;
        }

		public bool CanOccupy(GameObject obj)
		{
			return vacant && (m_reserver == null || ReservedBy(obj));
		}

		public bool OccupiedBy(GameObject obj)
        {
            return Equals(m_occupant, obj);
        }

        public bool ReservedBy(GameObject obj)
        {
            return Equals(m_reserver, obj);
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