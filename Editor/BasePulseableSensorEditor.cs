using UnityEngine;
using UnityEditor;
using ToolkitEngine.Sensors;

namespace ToolkitEditor.Sensors
{
    [CustomEditor(typeof(BasePulseableSensor))]
    public class BasePulseableSensorEditor : BaseDetectionSensorEditor
    {
        #region Fields

        protected SerializedProperty m_pulse;
        protected SerializedProperty m_pulseMode;

        // Events
        protected SerializedProperty m_onPulsed;

        #endregion

        #region Methods

        protected override void OnEnable()
        {
            base.OnEnable();

            m_pulse = serializedObject.FindProperty(nameof(m_pulse));
            m_pulseMode = m_pulse.FindPropertyRelative("pulseMode");

            // Events
            m_onPulsed = serializedObject.FindProperty(nameof(m_onPulsed));
        }

        protected override void OnCustomInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_pulseMode);
            base.OnCustomInspectorGUI();
        }

        protected override void OnSignalsInspectorGUI()
        {
            var sensor = target as BasePulseableSensor;
            if (sensor.pulseMode == PulseMode.Manual)
            {
                EditorGUILayout.Separator();
                if (GUILayout.Button("Pulse"))
                {
                    sensor.Pulse();
                }
            }

            base.OnSignalsInspectorGUI();
        }

        protected override void OnEventInspectorGUI()
        {
            base.OnEventInspectorGUI();
            EditorGUILayout.PropertyField(m_onPulsed);
        }

        #endregion
    }
}