using UnityEditor;
using ToolkitEngine.Sensors;

namespace ToolkitEditor.Sensors
{
    [CustomEditor(typeof(RaySensor))]
    public class RaySensorEditor : BasePulseableSensorEditor
    {
        #region Fields

        protected SerializedProperty m_direction;
        protected SerializedProperty m_space;
        protected SerializedProperty m_length;
        protected SerializedProperty m_radius;
        protected SerializedProperty m_blockingLayers;

        #endregion

        #region Methods

        protected override void OnEnable()
        {
            base.OnEnable();
            m_direction = serializedObject.FindProperty("direction");
            m_space = serializedObject.FindProperty("space");
            m_length = serializedObject.FindProperty("length");
            m_radius = serializedObject.FindProperty("radius");
            m_blockingLayers = serializedObject.FindProperty("blockingLayers");
        }

        protected override void OnCustomInspectorGUI()
        {
            base.OnCustomInspectorGUI();
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_direction);
            EditorGUILayout.PropertyField(m_space);
            EditorGUILayout.PropertyField(m_length);
            EditorGUILayout.PropertyField(m_radius);
        }

        protected override void OnDetectionInspectorGUI()
        {
            DrawDetectionMode();
            EditorGUILayout.PropertyField(m_detectionLayers);
            EditorGUILayout.PropertyField(m_blockingLayers);
            EditorGUILayout.PropertyField(m_queryTrigger);
        }

        #endregion
    }
}