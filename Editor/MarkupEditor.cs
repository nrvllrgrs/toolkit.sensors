using UnityEditor;
using ToolkitEngine.Sensors;

namespace ToolkitEditor.Sensors
{
	[CustomEditor(typeof(Markup))]
    public class MarkupEditor : BaseToolkitEditor
    {
		#region Fields

		protected SerializedProperty m_type;
		protected SerializedProperty m_radius;
		protected SerializedProperty m_height;

		// Events
		protected SerializedProperty m_onFirstDetection;
		protected SerializedProperty m_onSignalDetected;
		protected SerializedProperty m_onSignalUndetected;
		protected SerializedProperty m_onLastUndetection;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_type = serializedObject.FindProperty(nameof(m_type));
			m_radius = serializedObject.FindProperty(nameof(m_radius));
			m_height = serializedObject.FindProperty(nameof(m_height));

			// Events
			m_onFirstDetection = serializedObject.FindProperty(nameof (m_onFirstDetection));
			m_onSignalDetected = serializedObject.FindProperty(nameof(m_onSignalDetected));
			m_onSignalUndetected = serializedObject.FindProperty(nameof(m_onSignalUndetected));
			m_onLastUndetection = serializedObject.FindProperty(nameof(m_onLastUndetection));
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_type);
			EditorGUILayout.PropertyField(m_radius);
			EditorGUILayout.PropertyField(m_height);
		}

		protected override void DrawEvents()
		{
			if (EditorGUILayoutUtility.Foldout(m_onSignalDetected, "Events"))
			{
				EditorGUILayout.PropertyField(m_onFirstDetection);
				EditorGUILayout.PropertyField(m_onSignalDetected);
				EditorGUILayout.PropertyField(m_onSignalUndetected);
				EditorGUILayout.PropertyField(m_onLastUndetection);

				DrawNestedEvents();
			}
		}

		#endregion
	}
}