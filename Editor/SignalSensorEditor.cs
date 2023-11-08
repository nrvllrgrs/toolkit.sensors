using UnityEngine;
using UnityEditor;
using ToolkitEngine.Sensors;

namespace ToolkitEditor.Sensors
{
    [CustomEditor(typeof(SignalSensor), true)]
    public class SignalSensorEditor : BaseSensorEditor
    {
        #region Fields

        protected SerializedProperty m_immediate;
        protected SerializedProperty m_radius;
        protected SerializedProperty m_validSignals;

        protected SerializedProperty m_forgetMode;
        protected SerializedProperty m_forgetTime;
        protected SerializedProperty m_forgetRate;

        #endregion

        #region Methods

        protected override void OnEnable()
        {
            base.OnEnable();
            m_immediate = serializedObject.FindProperty("immediate");
            m_radius = serializedObject.FindProperty("radius");
            m_validSignals = serializedObject.FindProperty("validSignals");

            m_forgetMode = serializedObject.FindProperty("forgetMode");
            m_forgetTime = serializedObject.FindProperty("forgetTime");
            m_forgetRate = serializedObject.FindProperty("forgetRate");
        }

        protected override void OnCustomInspectorGUI()
        {
            base.OnCustomInspectorGUI();
            EditorGUILayout.PropertyField(m_immediate);

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_forgetMode);

            ++EditorGUI.indentLevel;
            switch ((SignalSensor.ForgetMode)m_forgetMode.intValue)
            {
                case SignalSensor.ForgetMode.Time:
                    EditorGUILayout.PropertyField(m_forgetTime, new GUIContent("Time"));
                    break;

                case SignalSensor.ForgetMode.Rate:
                    EditorGUILayout.PropertyField(m_forgetRate, new GUIContent("Rate"));
                    break;
            }
            --EditorGUI.indentLevel;

            EditorGUILayout.PropertyField(m_radius);
            EditorGUILayout.PropertyField(m_validSignals);
        }

        #endregion
    }
}