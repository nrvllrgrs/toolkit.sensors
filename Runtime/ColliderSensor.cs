using UnityEngine;

namespace ToolkitEngine.Sensors
{
    [AddComponentMenu("Sensor/Collider Sensor")]
    public class ColliderSensor : BaseColliderSensor
    {
        #region Methods

        private void OnCollisionEnter(Collision collision)
        {
            Add(collision.collider);
        }

        private void OnCollisionExit(Collision collision)
        {
            Remove(collision.collider);
        }

        #endregion
    }
}