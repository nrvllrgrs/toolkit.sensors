using UnityEngine;

namespace ToolkitEngine.Sensors
{
    [AddComponentMenu("Sensor/Trigger Sensor")]
    public class TriggerSensor : BaseColliderSensor
    {
        #region Methods

        private void OnTriggerEnter(Collider other)
        {
            Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            Remove(other);
        }

        #endregion
    }
}