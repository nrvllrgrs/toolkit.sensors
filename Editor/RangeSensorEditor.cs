using UnityEditor;
using ToolkitEngine.Sensors;

namespace ToolkitEditor.Sensors
{
    [CustomEditor(typeof(RangeSensor))]
    public class RangeSensorEditor : BasePulseableSensorEditor
    {
        #region Fields

        protected SerializedProperty m_radius;

        #endregion

        #region Methods

        protected override void OnEnable()
        {
            base.OnEnable();
            m_radius = serializedObject.FindProperty("radius");
        }

        protected override void OnCustomInspectorGUI()
        {
            base.OnCustomInspectorGUI();
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_radius);
        }

        #endregion
    }
}