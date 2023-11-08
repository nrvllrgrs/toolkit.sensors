using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ToolkitEngine.Sensors
{
    [AddComponentMenu("Sensor/Ray Sensor")]
    public class RaySensor : BasePulseableSensor
    {
        #region Fields

        public Vector3 direction = Vector3.forward;
        public Space space = Space.Self;

        [Min(0f)]
        public float length = 5f;

        [Min(0f)]
        public float radius;

        public LayerMask blockingLayers = ~0;

        #endregion

        #region Methods

        protected override void CustomPulse()
        {
            Vector3 forward = direction.normalized;
            switch (space)
            {
                case Space.Self:
                    forward = transform.rotation * forward;
                    break;
            }

            LayerMask layers = detectionLayers | blockingLayers;

            // If detection is a subset of blocking layers, then only get first hit
            if ((detectionLayers & ~blockingLayers) == 0)
            {
                if (radius <= 0f)
                {
                    if (Physics.Raycast(transform.position, forward, out RaycastHit hit, length, layers, queryTrigger))
                    {
                        ProcessHit(hit);
                    }
                }
                else
                {
                    if (Physics.SphereCast(transform.position, radius, forward, out RaycastHit hit, length, layers, queryTrigger))
                    {
                        ProcessHit(hit);
                    }
                }
            }
            // Yep...all cast
            else
            {
                RaycastHit[] hits;
                if (radius <= 0f)
                {
                    hits = Physics.RaycastAll(transform.position, forward, length, layers, queryTrigger);
                }
                else
                {
                    hits = Physics.SphereCastAll(transform.position, radius, forward, length, layers, queryTrigger);
                }

                // RaycastAll and SphereCastAll results do not return in undefined order
                // Order by closest-to-farthest
                foreach (var hit in hits.OrderBy(x => (x.point - transform.position).sqrMagnitude))
                {
                    if (ProcessHit(hit))
                        break;
                }
            }
        }

        private bool ProcessHit(RaycastHit hit)
        {
            GameObject detected = null;
            switch (detectionMode)
            {
                case DetectionMode.Collider:
                    detected = hit.collider.gameObject;
                    break;

                case DetectionMode.Rigidbody:
                    detected = hit.rigidbody?.gameObject;
                    break;

                default:
                    detected = GetDetected(this, hit.collider);
                    break;
            }

            detected = GetFilteredObject(detected);

            // Hit object doesn't exist, skip
            if (detected == null)
                return false;

            // Detected
            if ((detectionLayers & (1 << hit.collider.gameObject.layer)) != 0)
            {
                m_pulse.AddPendingSignal(this, detected, hit.point);
            }

            // Blocked, stop processing
            return (blockingLayers & (1 << hit.collider.gameObject.layer)) != 0;
        }

        #endregion

        #region Editor-Only
#if UNITY_EDITOR

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Handles.color = Color.green;
            Vector3 forward = direction.normalized;
            Vector3 up = Vector3.up;

            // If vectors are parallel, find new up vector
            if (Vector3.Cross(forward, up) == Vector3.zero)
            {
                up = Vector3.forward;
            }

            switch (space)
            {
                case Space.Self:
                    forward = transform.rotation * forward;
                    up = transform.rotation * up;
                    break;
            }

            var far = transform.position + forward * length;
            if (radius > 0)
            {
                Handles.DrawWireDisc(transform.position, forward, radius);
                Handles.DrawWireDisc(far, forward, radius);

                var right = Vector3.Cross(forward, up);

                Gizmos.DrawLine(transform.position + (up * radius), far + (up * radius));
                Gizmos.DrawLine(transform.position + (up * -radius), far + (up * -radius));
                Gizmos.DrawLine(transform.position + (right * radius), far + (right * radius));
                Gizmos.DrawLine(transform.position + (right * -radius), far + (right * -radius));
            }
            else
            {
                Gizmos.DrawLine(transform.position, far);
            }
        }

#endif
        #endregion
    }
}