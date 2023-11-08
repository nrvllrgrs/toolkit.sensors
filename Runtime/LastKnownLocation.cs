using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

namespace ToolkitEngine.Sensors
{
    public class LastKnownLocation : MonoBehaviour
    {
        #region Fields

        [SerializeField, Required]
        private BaseSensor m_sensor;

        [SerializeField, Min(0f)]
        private float m_timeToForget = 10f;

        private Dictionary<GameObject, Location> m_forgetLocations = new();
        private Coroutine m_forgetThread = null;

        #endregion

        #region Properties

        public GameObject[] KnownGameObjects
        {
            get
            {
                // Get objects from sensor and gameObjects to forget
                return m_sensor.signals.Select(x => x.detected)
                    .Concat(m_forgetLocations.Keys).ToArray();
            }
        }

        #endregion

        #region Methods

        private void OnEnable()
        {
            m_sensor.onSignalDetected.AddListener(SignalDetected);
            m_sensor.onSignalUndetected.AddListener(SignalUndetected);
        }

        private void OnDisable()
        {
            m_sensor.onSignalDetected.RemoveListener(SignalDetected);
            m_sensor.onSignalUndetected.RemoveListener(SignalUndetected);
        }

        private void SignalDetected(SensorEventArgs e)
        {
            if (m_forgetLocations.ContainsKey(e.signal.detected))
            {
                m_forgetLocations.Remove(e.signal.detected);

                // If there is nothing to forget, cancel coroutine
                if (m_forgetLocations.Count == 0)
                {
                    this.CancelCoroutine(ref m_forgetThread);
                }
            }
        }

        private void SignalUndetected(SensorEventArgs e)
        {
            // Immediately forget location, skip
            if (m_timeToForget <= 0f)
                return;

            m_forgetLocations.Add(e.signal.detected, new Location()
            {
                point = e.signal.detected.transform.position,
                remainingTime = m_timeToForget
            });

            if (m_forgetThread == null)
            {
                m_forgetThread = StartCoroutine(AsyncForgetLocation());
            }
        }

        public bool TryGetLocation(GameObject obj, out Vector3 point)
        {
            if (m_forgetLocations.TryGetValue(obj, out Location value))
            {
                point = value.point;
                return true;
            }

            if (m_sensor.IsDetecting(obj))
            {
                point = m_sensor.transform.position;
                return true;
            }

            point = default;
            return false;
        }

        private IEnumerator AsyncForgetLocation()
        {
            HashSet<GameObject> keysToRemove = new();

            while (true)
            {
                keysToRemove.Clear();

                foreach (var p in m_forgetLocations)
                {
                    p.Value.remainingTime -= Time.deltaTime;

                    if (p.Value.remainingTime <= 0f)
                    {
                        keysToRemove.Add(p.Key);
                    }
                }

                // Remove locations that haven't been detected for awhile
                foreach (var key in keysToRemove)
                {
                    m_forgetLocations.Remove(key);
                }

                // Wait a frame
                yield return null;
            }
        }

        #endregion

        #region Structures

        private class Location
        {
            public Vector3 point;
            public float remainingTime;
        }

        #endregion
    }
}