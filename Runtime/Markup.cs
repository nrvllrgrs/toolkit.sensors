using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.Sensors
{
    public class Markup : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private MarkupType m_type;

		private GameObject m_reserver;
		private GameObject m_occupant;

        #endregion

        #region Events

        [SerializeField]
        private UnityEvent<SensorEventArgs> m_onSignalDetected;

		[SerializeField]
		private UnityEvent<SensorEventArgs> m_onSignalUndetected;

		#endregion

		#region Properties

		public MarkupType type => m_type;

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
        /// Invoked when this Markup is detected by any MarkupSensor
        /// </summary>
        public UnityEvent<SensorEventArgs> onSignalDetected => m_onSignalDetected;

        /// <summary>
        /// Invoked when this Markup is undetected by any MarkupSensor
        /// </summary>
        public UnityEvent<SensorEventArgs> onSignalUndetected => m_onSignalUndetected;

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

        public virtual bool Reserve(GameObject obj)
        {
            // Already occupied, cannot reserve
            if (!vacant)
                return false;

            m_reserver = obj;
            return true;
		}

        public virtual bool Cancel(GameObject obj)
        {
            if (!ReservedBy(obj))
                return false;

            m_reserver = null;
            return true;
        }

        public void ForceCancel()
        {
            m_reserver = null;
        }

        public virtual bool Arrive(GameObject obj)
        {
            if (!CanOccupy(obj))
                return false;

            m_reserver = null;
            m_occupant = obj;
            return true;
        }

        public virtual bool Depart(GameObject obj)
        {
            if (!OccupiedBy(obj))
                return false;

            m_occupant = null;
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
    }
}