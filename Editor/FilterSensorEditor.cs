using UnityEditor;
using ToolkitEngine.Sensors;
using UnityEngine;
using System.Data.Odbc;

namespace ToolkitEditor.Sensors
{
    [CustomEditor(typeof(FilterSensor))]
    public class FilterSensorEditor : BaseSensorEditor
    {
        #region Fields

        protected SerializedProperty m_sensor;
        protected SerializedProperty m_autoPulse;

        protected SerializedProperty m_pulse;
        protected SerializedProperty m_pulseMode;

        // Events
        protected SerializedProperty m_onPulsed;

        #endregion

        #region Methods

        protected override void OnEnable()
        {
            base.OnEnable();
            m_sensor = serializedObject.FindProperty(nameof(m_sensor));
            m_autoPulse = serializedObject.FindProperty(nameof(m_autoPulse));

            m_pulse = serializedObject.FindProperty(nameof(m_pulse));
            m_pulseMode = m_pulse.FindPropertyRelative("pulseMode");
        }

        protected override void OnCustomInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_pulseMode);
            base.OnCustomInspectorGUI();

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_sensor);

            if (m_sensor.objectReferenceValue is IPulseableSensor)
            {
                ++EditorGUI.indentLevel;
                EditorGUILayout.PropertyField(m_autoPulse);
				--EditorGUI.indentLevel;
			}
        }

        protected override void OnEventInspectorGUI()
        {
            base.OnEventInspectorGUI();
            EditorGUILayout.PropertyField(m_onPulsed);
        }

        #endregion
    }
}