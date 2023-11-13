using UnityEditor;
using ToolkitEngine.Sensors;

namespace ToolkitEditor.Sensors
{
	[CustomEditor(typeof(Markup))]
    public class MarkupEditor : BaseToolkitEditor
    {
		#region Fields

		protected SerializedProperty m_type;

		// Events
		protected SerializedProperty m_onSignalDetected;
		protected SerializedProperty m_onSignalUndetected;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_type = serializedObject.FindProperty(nameof(m_type));

			// Events
			m_onSignalDetected = serializedObject.FindProperty(nameof(m_onSignalDetected));
			m_onSignalUndetected = serializedObject.FindProperty(nameof(m_onSignalUndetected));
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_type);
		}

		protected override void DrawEvents()
		{
			if (EditorGUILayoutUtility.Foldout(m_onSignalDetected, "Events"))
			{
				EditorGUILayout.PropertyField(m_onSignalDetected);
				EditorGUILayout.PropertyField(m_onSignalUndetected);

				DrawNestedEvents();
			}
		}

		#endregion
	}
}