using UnityEngine;

namespace ToolkitEngine.Sensors
{
    [AddComponentMenu("Sensor/LOS Targets")]
    public class LOSTargets : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private Target[] m_targets;

        #endregion

        #region Properties

        public Target[] Targets => m_targets;

        #endregion

        #region Structures

        [System.Serializable]
        public struct Target
        {
            public Transform Point;

            [Range(0f, 1f)]
            public float Weight;
        }

        #endregion
    }
}