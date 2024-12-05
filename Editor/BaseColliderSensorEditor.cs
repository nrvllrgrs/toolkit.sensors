using UnityEditor;
using ToolkitEngine.Sensors;

namespace ToolkitEditor.Sensors
{
    [CustomEditor(typeof(BaseColliderSensor), true)]
    public class BaseColliderSensorEditor : BaseDetectionSensorEditor
    {
        #region Fields

        protected SerializedProperty m_isOn;
        protected SerializedProperty m_stopOnDetection;

        #endregion

        #region Methods

        protected override void OnEnable()
        {
            base.OnEnable();
            m_isOn = serializedObject.FindProperty(nameof(m_isOn));
            m_stopOnDetection = serializedObject.FindProperty(nameof(m_stopOnDetection));
        }

        protected override void OnCustomInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_isOn);
            EditorGUILayout.PropertyField(m_stopOnDetection);
            base.OnCustomInspectorGUI();
        }

        protected override void OnDetectionInspectorGUI()
        {
            DrawDetectionMode();
        }

        #endregion
    }
}