using UnityEngine;

namespace ToolkitEngine.Sensors
{
    public enum DetectionMode
    {
        Collider,
        Rigidbody,
        CharacterController,
        Custom,
    }

    public abstract class BaseDetectionSensor : BaseSensor
    {
        #region Fields

        [SerializeField]
        private DetectionMode m_detectionMode = DetectionMode.Collider;

        [SerializeField]
        protected string m_componentName;

        public LayerMask detectionLayers = ~0;
        public QueryTriggerInteraction queryTrigger = QueryTriggerInteraction.Ignore;

        #endregion

        #region Properties

        public virtual DetectionMode detectionMode => m_detectionMode;

        #endregion

        #region Methods

        public static GameObject GetDetected(BaseDetectionSensor sensor, Collider collider)
        {
            if (collider == null)
                return null;

            switch (sensor.m_detectionMode)
            {
                case DetectionMode.Collider:
                    return collider.gameObject;

                case DetectionMode.Rigidbody:
                    return collider.GetComponentInParent<Rigidbody>()?.gameObject;

                case DetectionMode.CharacterController:
                    return collider.GetComponentInParent<CharacterController>()?.gameObject;

                case DetectionMode.Custom:
                    Transform transform = collider.transform;
                    while (transform != null)
                    {
                        var component = transform.GetComponent(sensor.m_componentName);
                        if (component != null)
                        {
                            return component.gameObject;
                        }

                        transform = transform.parent;
                    }
                    return null;
            }
            return null;
        }

        #endregion
    }
}