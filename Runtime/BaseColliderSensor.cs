using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine.Sensors
{
    public abstract class BaseColliderSensor : BaseDetectionSensor
    {
        #region Fields

        [SerializeField]
        protected bool m_isOn = true;

        private Dictionary<Collider, int> m_inSet = new();
        private Dictionary<Collider, GameObject> m_map = new();

        #endregion

        #region Properties

        public bool isOn
        {
            get => m_isOn;
            set
            {
                if (m_isOn == value)
                    return;

                m_isOn = value;

                if (value)
                {
                    foreach (var p in m_inSet)
                    {
                        AddSignal(p.Key);
                    }
                }
                else
                {
                    m_map.Clear();
                    ClearSignals();
                }
            }
        }

        #endregion

        #region Methods

        private void OnDisable()
        {
            m_inSet.Clear();
        }

        protected void Add(Collider other)
        {
            if (!m_inSet.ContainsKey(other))
            {
				m_inSet.Add(other, 1);
			}
            else
            {
                ++m_inSet[other];
            }

            if (!isOn || m_map.ContainsKey(other))
                return;

            AddSignal(other);
        }

        protected void Remove(Collider other)
        {
            if (m_inSet.TryGetValue(other, out int count))
            {
                --count;
                if (count == 0)
                {
                    m_inSet.Remove(other);
                }
                else
                {
                    m_inSet[other] = count;
                }
            }

            if (isOn && !m_inSet.ContainsKey(other) && m_map.TryGetValue(other, out GameObject detected))
            {
                m_map.Remove(other);
                RemoveSignal(detected);
            }
        }

        protected void AddSignal(Collider collider)
        {
            var detected = GetFilteredObject(GetDetected(this, collider));
            if (detected == null)
                return;

            m_map.Add(collider, detected);
            AddSignal(detected);
        }

        #endregion
    }
}