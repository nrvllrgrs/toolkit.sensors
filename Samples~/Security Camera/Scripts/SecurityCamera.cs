using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using DG.Tweening;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ToolkitEngine.Sensors
{
    public class SecurityCamera : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private Perception m_perception;

        [SerializeField]
        private Vector3 m_targetOffset;

        [SerializeField]
        private bool m_useTargetingPoints = false;

        [Space]

        [SerializeField, Min(0f)]
        private float m_speed = 45f;

        [SerializeField]
        private AnimationCurve m_ease = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Space]

        [SerializeField, Tooltip("Seconds paused when arriving at a patrol angle."), Min(0f)]
        private float m_pauseTime = 3f;

        [SerializeField, Tooltip("Seconds paused when losing target"), Min(0f)]
        private float m_recoveryTime = 3f;

        [Space]

        [SerializeField]
        private HorizVertAngles[] m_patrolAngles;

        [Header("Head Settings")]

        [SerializeField]
        private Transform m_horizPivot;

        [SerializeField, MinMaxSlider(-180f, 180f)]
        private Vector2 m_horizLimits = new Vector2(-90f, 90f);

		[SerializeField]
		private Transform m_vertPivot;

		[SerializeField, MinMaxSlider(-90, 90f)]
        private Vector2 m_vertLimits = new Vector2(-45f, 0f);

        private int m_index;
        private bool m_forward = true;
        private Tween m_tween;

        private Coroutine m_trackingThread = null;

        #endregion

        #region Properties

        public bool isTracking => m_trackingThread != null;

        //public bool isAlert
        //{
        //    get => m_isAlert;
        //    private set
        //    {
        //        // No change, skip
        //        if (m_isAlert == value)
        //            return;

        //        m_isAlert = value;
        //        if (value)
        //        {
        //            m_onAlert.Invoke();
        //        }
        //        else
        //        {
        //            m_onSearch.Invoke();
        //        }
        //    }
        //}

        #endregion

        #region Methods

        private void Awake() 
        {
            m_perception = m_perception ?? GetComponent<Perception>();
            Assert.IsNotNull(m_perception);

            // Snap to initial position
            m_horizPivot.localRotation = GetTargetLocalRotation(GetForward());
        }

        private void OnEnable()
        {
            m_perception.onStimulusSensed.AddListener(Perception_Disturbance);
            m_perception.onStimulusUnknown.AddListener(Perception_Unknown);

            // Start searching
            PathComplete();
        }

        private void OnDisable()
        {
            m_perception.onStimulusSensed.RemoveListener(Perception_Disturbance);
            m_perception.onStimulusUnknown.RemoveListener(Perception_Unknown);

            m_tween.Kill(false);
            this.CancelCoroutine(ref m_trackingThread);
        }

        private void PathComplete()
        {
            PathComplete(Next(), m_pauseTime);
        }

        private void PathComplete(Vector3 forward, float delay)
        {
            var target = GetTargetLocalRotation(forward).eulerAngles;
			if (m_horizPivot == m_vertPivot)
            {
                m_tween = m_horizPivot.DOLocalRotate(target, m_speed);
			}
            else
            {
                // Sequences do not support speed-based
                // Need to calculate mximum duration
                var horizTarget = new Vector3(0f, target.y, 0f);
				var vertTarget = new Vector3(target.x, 0f, 0f);

                float duration = Mathf.Max(
                    GetTweenDuration(new Vector3(0f, m_horizPivot.localEulerAngles.y, 0f), horizTarget),
                    GetTweenDuration(new Vector3(m_vertPivot.localEulerAngles.x, 0f, 0f), vertTarget));

				m_tween = DOTween.Sequence()
                    .Append(m_horizPivot.DOLocalRotate(horizTarget, duration))
                    .Join(m_vertPivot.DOLocalRotate(vertTarget, duration));
            }

			SetupTween(m_tween, delay);
		}

        private float GetTweenDuration(Vector3 localEulerAngles, Vector3 targetForward)
        {
			return Vector3.Angle(GetTargetLocalRotation(localEulerAngles).eulerAngles, targetForward) / m_speed;
		}

        private Tween SetupTween(Tween tween, float delay)
        {
            return tween.SetEase(m_ease)
                .SetDelay(delay)
                .SetSpeedBased(true)
                .OnComplete(PathComplete);
		}

        private Vector3 Next()
        {
            if (!m_forward)
            {
                // At beginning, go forwards
                if (m_index == 0)
                {
                    m_forward = true;
                }
            }
            // At end, go backwards
            else if (m_index == m_patrolAngles.Length - 1)
            {
                m_forward = false;
            }

            // Advance index
            m_index += m_forward ? 1 : -1;
            return GetForward();
        }

        private Vector3 GetForward() => GetForward(m_patrolAngles[m_index]);

        private Vector3 GetForward(HorizVertAngles angles) => Quaternion.AngleAxis(angles.horiz, transform.up) * Quaternion.AngleAxis(angles.vert, transform.right) * transform.forward;

        private void Perception_Disturbance(PerceptionEventArgs e)
        {
            // Stop tweening
            m_tween.Kill();

            // Start tracking target within limits
            m_trackingThread = StartCoroutine(AsyncTracking());
        }

        private void Perception_Unknown(PerceptionEventArgs e)
        {
            if (m_trackingThread != null)
            {
                StopCoroutine(m_trackingThread);
                m_trackingThread = null;
            }

            HorizVertAngles minAngles = default;
            float minDelta = float.MaxValue;

            // Find min angle
            foreach (var angles in m_patrolAngles)
            {
                float delta = Vector3.Angle(m_horizPivot.forward, GetForward(angles));
                if (delta < minDelta)
                {
                    minAngles = angles;
                    minDelta = delta;
                }
            }

            // Start moving back to closet angle on path
            PathComplete(GetForward(minAngles), m_recoveryTime);
        }

        private Quaternion GetTargetLocalRotation(Vector3 forward) => Quaternion.Inverse(transform.rotation) * Quaternion.LookRotation(forward);

        private IEnumerator AsyncTracking()
        {
            while (true)
            {
                if (m_perception.target != null)
                {
                    Transform target = m_perception.target.transform;
                    if (m_useTargetingPoints)
                    {
                        var targetingPoints = m_perception.target.GetComponent<TargetingPoints>();
                        if (targetingPoints != null)
                        {
                            target = targetingPoints[0];
                        }
                    }

                    if (m_horizPivot == m_vertPivot)
                    {
                        // Clamp rotation between limits
                        // Wrap angle to ensure angles between -180 and 180 degrees
                        var nextEulerAngles = GetTargetLocalRotation(GetNextForward(m_horizPivot, target.position)).eulerAngles.WrapEulerAngles();
                        nextEulerAngles.x = Mathf.Clamp(nextEulerAngles.x, m_vertLimits.x, m_vertLimits.y);
                        nextEulerAngles.y = Mathf.Clamp(nextEulerAngles.y, m_horizLimits.x, m_horizLimits.y);

                        // Set rotation
                        m_horizPivot.localEulerAngles = nextEulerAngles;
                    }
                    else
                    {
                        // Calculate new forwards before applying rotation
						var nextHorizEulerAngles = GetTargetLocalRotation(GetNextForward(m_horizPivot, target.position)).eulerAngles.WrapEulerAngles();
						var nextVertEulerAngles = GetTargetLocalRotation(GetNextForward(m_vertPivot, target.position)).eulerAngles.WrapEulerAngles();

						// Set rotation
						m_horizPivot.localEulerAngles = new Vector3(0f, Mathf.Clamp(nextHorizEulerAngles.y, m_horizLimits.x, m_horizLimits.y), 0f);
						m_vertPivot.localEulerAngles = new Vector3(Mathf.Clamp(nextVertEulerAngles.x, m_vertLimits.x, m_vertLimits.y), 0f, 0f);
					}
                }

                // Wait a frame
                yield return null;
            }
        }

        private Vector3 GetNextForward(Transform pivot, Vector3 target)
        {
            return Vector3.RotateTowards(
                pivot.forward,
                (target + m_targetOffset - pivot.position).normalized,
                m_speed * Mathf.Deg2Rad * Time.deltaTime, 0f);

		}


        #endregion

        #region Editor-Only
#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (m_horizPivot == null || m_vertPivot == null)
                return;

            DrawArc(m_horizPivot, m_horizLimits, transform.up, Color.green);
            DrawArc(m_vertPivot, m_vertLimits, transform.right, Color.red);

            Gizmos.color = Handles.color = Color.white;
            for (int i = 0; i < m_patrolAngles.Length - 1; ++i)
            {
                var from = GetForward(m_patrolAngles[i]);
                var to = GetForward(m_patrolAngles[i + 1]);
                var normal = Vector3.Cross(to, from);

                Handles.DrawWireArc(m_vertPivot.transform.position, normal, from, Vector3.SignedAngle(from, to, normal), 1f);
                Gizmos.DrawRay(m_vertPivot.transform.position, from);
                Gizmos.DrawRay(m_vertPivot.transform.position, to);
            }
        }

        private void DrawArc(Transform pivot, Vector2 limits, Vector3 normal, Color color)
        {
            Gizmos.color = Handles.color = color;

            var from = Quaternion.AngleAxis(limits.x, normal) * transform.forward;
            Handles.DrawWireArc(pivot.transform.position, normal, from, limits.y - limits.x, 1f);
            Gizmos.DrawRay(pivot.transform.position, from);
            Gizmos.DrawRay(pivot.transform.position, Quaternion.AngleAxis(limits.y, normal) * transform.forward);
        }

#endif
        #endregion

        #region Structures

        [System.Serializable]
        private struct HorizVertAngles
        {
            [Range(-90f, 90f)]
            public float horiz;

            [Range(-90f, 90f)]
            public float vert;
        }

        #endregion
    }
}