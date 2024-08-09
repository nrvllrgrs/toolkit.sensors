using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.Sensors
{
    [AddComponentMenu("Sensor/Trigger Sensor")]
    public class TriggerSensor : BaseColliderSensor
    {
        #region Fields

        private bool m_enteredFromTail = false;

		#endregion

		#region Events

		[SerializeField, Tooltip("Invoked when a signal is detected from tail of forward vector.")]
		private UnityEvent<SensorEventArgs> m_onDetectedTail = new UnityEvent<SensorEventArgs>();

		[SerializeField, Tooltip("Invoked when a signal is detected from tip of forward vector.")]
		private UnityEvent<SensorEventArgs> m_onDetectedTip = new UnityEvent<SensorEventArgs>();

		[SerializeField, Tooltip("Invoked when a signal is undetected from tail of forward vector.")]
		private UnityEvent<SensorEventArgs> m_onUndetectedTail = new UnityEvent<SensorEventArgs>();

		[SerializeField, Tooltip("Invoked when a signal is undetected from tip of forward vector.")]
		private UnityEvent<SensorEventArgs> m_onUndetectedTip = new UnityEvent<SensorEventArgs>();

		[SerializeField, Tooltip("Invoked when a signal is detected from tail and undetected from tip of forward vector.")]
		private UnityEvent<SensorEventArgs> m_onUndetectedTailToTip = new UnityEvent<SensorEventArgs>();

		[SerializeField, Tooltip("Invoked when a signal is detected from tip and undetected from tail of forward vector.")]
		private UnityEvent<SensorEventArgs> m_onUndetectedTipToTail = new UnityEvent<SensorEventArgs>();

		#endregion

		#region Properties

		public UnityEvent<SensorEventArgs> onDetectedTail => m_onDetectedTail;
		public UnityEvent<SensorEventArgs> onDetectedTip => m_onDetectedTip;
		public UnityEvent<SensorEventArgs> onUndetectedTail => m_onUndetectedTail;
		public UnityEvent<SensorEventArgs> onUndetectedTip => m_onUndetectedTip;
		public UnityEvent<SensorEventArgs> onUndetectedTailToTip => m_onUndetectedTailToTip;
		public UnityEvent<SensorEventArgs> onUndetectedTipToTail => m_onUndetectedTipToTail;

		#endregion

		#region Methods

		private void OnTriggerEnter(Collider other)
        {
            Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            Remove(other);
        }

        protected override void CustomAddSignal(SensorEventArgs args)
        {
			if (GetDot(args) < 0)
			{
				m_enteredFromTail = true;
				m_onDetectedTail?.Invoke(args);
			}
			else
			{
				m_enteredFromTail = false;
				m_onDetectedTip?.Invoke(args);
			}
        }

        protected override void CustomRemoveSignal(SensorEventArgs args)
        {
			if (GetDot(args) < 0)
			{
				m_onUndetectedTail?.Invoke(args);
				if (!m_enteredFromTail)
				{
					m_onUndetectedTipToTail?.Invoke(args);
				}
			}
			else
			{
				m_onUndetectedTip?.Invoke(args);
				if (m_enteredFromTail)
				{
					m_onUndetectedTailToTip?.Invoke(args);
				}
			}
        }

        private float GetDot(SensorEventArgs args) => Vector3.Dot((args.signal.position - transform.position).normalized, transform.forward);

		#endregion
	}
}