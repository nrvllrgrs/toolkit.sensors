using UnityEditor;
using ToolkitEngine.Sensors;

namespace ToolkitEditor.Sensors
{
    [CustomEditor(typeof(BaseColliderSensor), true)]
    public class BaseColliderSensorEditor : BaseDetectionSensorEditor
    {
        #region Fields

        protected SerializedProperty m_isOn;

        #endregion

        #region Methods

        protected override void OnEnable()
        {
            base.OnEnable();
            m_isOn = serializedObject.FindProperty(nameof(m_isOn));
        }

        protected override void OnCustomInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_isOn);
            base.OnCustomInspectorGUI();
        }

        protected override void OnDetectionInspectorGUI()
        {
            DrawDetectionMode();
        }

        #endregion
    }
}