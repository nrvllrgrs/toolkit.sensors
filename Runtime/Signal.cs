using UnityEngine;

namespace ToolkitEngine.Sensors
{
    [System.Serializable]
    public class Signal
    {
        #region Fields

        public GameObject detected;
        public float strength;
        public SignalType type;

        private Vector3? m_position;

        #endregion

        #region Events

        public static event System.EventHandler<SensorEventArgs> Broadcasted;

        #endregion

        #region Properties

        public Vector3 position => m_position.HasValue ? m_position.Value : Vector3.zero;

        #endregion

        #region Constructors

        public Signal(GameObject detected, float strength = 1f, SignalType signalType = null)
        {
            this.detected = detected;
            this.strength = strength;
            type = signalType;

            m_position = detected.transform.position;
        }

        public Signal(GameObject detected, Vector3 position, float strength = 1f, SignalType signalType = null)
            : this(detected, strength, signalType)
        {
            m_position = position;
        }

        public Signal(Signal other)
            : this(other.detected, other.position, other.strength, other.type)
        { }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return GetHashCode().Equals(obj.GetHashCode());
        }

        public override int GetHashCode() => detected?.GetHashCode() ?? 0;

        #endregion

        #region Static Methods

        public static void Broadcast(GameObject source, Vector3 position, float strength, SignalType signalType = null)
        {
            Broadcasted?.Invoke(source, new SensorEventArgs()
            {
                sensor = null,
                signal = new Signal(source, position, strength, signalType)
            });
        }

        public static void Broadcast(GameObject source, Vector3 position, SignalType signalType = null)
        {
            Broadcast(source, position, 1f, signalType);
        }

        #endregion
    }
}