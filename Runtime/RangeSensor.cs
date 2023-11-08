using UnityEngine;

namespace ToolkitEngine.Sensors
{
    [AddComponentMenu("Sensor/Range Sensor")]
    public class RangeSensor : BasePulseableSensor
    {
        #region Fields

        [Min(0f)]
        public float radius = 5f;

        #endregion

        #region Methods

        protected override void CustomPulse()
        {
            // Check detection mode outside of the loop (only need to do one if-check)
            // Then loop using a known method to get detected object
            switch (detectionMode)
            {
                case DetectionMode.Collider:
                    Pulse((collider) => collider.gameObject);
                    break;

                case DetectionMode.Rigidbody:
                    Pulse((collider) => collider.GetComponentInParent<Rigidbody>()?.gameObject);
                    break;

                case DetectionMode.CharacterController:
                    Pulse((collider) => collider.GetComponentInParent<CharacterController>()?.gameObject);
                    break;

                case DetectionMode.Custom:
                    Pulse((collider) =>
                    {
                        Transform transform = collider.transform;
                        while (transform != null)
                        {
                            var component = collider.GetComponent(m_componentName);
                            if (component != null)
                            {
                                return component.gameObject;
                            }

                            transform = transform.parent;
                        }
                        return null;
                    });
                    break;
            }
        }

        private void Pulse(System.Func<Collider, GameObject> getDetected)
        {
            foreach (var collider in Physics.OverlapSphere(transform.position, radius, detectionLayers, queryTrigger))
            {
                var detected = GetFilteredObject(getDetected(collider));
                if (detected == null)
                    continue;

                m_pulse.AddPendingSignal(this, detected);
            }
        }

        #endregion

        #region Editor-Only
#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

#endif
        #endregion
    }
}