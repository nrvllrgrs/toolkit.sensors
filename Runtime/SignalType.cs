using UnityEngine;

namespace ToolkitEngine.Sensors
{
    [CreateAssetMenu(menuName = "Toolkit/Sensors/Signal")]
    public class SignalType : ScriptableObject
    {
        #region Fields

        [SerializeField]
        private SignalType m_parent;

        #endregion

        #region Properties

        public SignalType Parent => m_parent;

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether item is a subtype of specified SignalType
        /// </summary>
        /// <param name="signalType">SignalType to compare with current type</param>
        /// <returns>Returns true if current type is descendant of signalType; otherwise false</returns>
        public bool IsSubtypeOf(SignalType signalType)
        {
            if (signalType == null)
                return false;

            var type = this;
            while (type != null)
            {
                if (type == signalType)
                    return true;

                type = type.m_parent;
            }

            return false;
        }

        /// <summary>
        /// Determines whether item is a supertype of specified SignalType
        /// </summary>
        /// <param name="signalType">SignalType to compare with current type</param>
        /// <returns>Returns true if current type is ancestor of signalType; otherwise false</returns>
        public bool IsSupertypeOf(SignalType signalType)
        {
            if (signalType == null)
                return false;

            return signalType.IsSubtypeOf(this);
        }

        #endregion
    }
}