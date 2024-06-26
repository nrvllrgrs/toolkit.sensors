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
		protected SerializedProperty m_onArrival;
		protected SerializedProperty m_onDeparture;
		protected SerializedProperty m_onReserved;
		protected SerializedProperty m_onCanceled;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_type = serializedObject.FindProperty(nameof(m_type));

			// Events
			m_onSignalDetected = serializedObject.FindProperty(nameof(m_onSignalDetected));
			m_onSignalUndetected = serializedObject.FindProperty(nameof(m_onSignalUndetected));
			m_onArrival = serializedObject.FindProperty(nameof(m_onArrival));
			m_onDeparture = serializedObject.FindProperty(nameof(m_onDeparture));
			m_onReserved = serializedObject.FindProperty(nameof(m_onReserved));
			m_onCanceled = serializedObject.FindProperty(nameof(m_onCanceled));
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_type);
		}

		protected override void DrawEvents()
		{
			if (EditorGUILayoutUtility.Foldout(m_onSignalDetected, "Events"))
			{
				EditorGUILayout.LabelField("Signal", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(m_onSignalDetected);
				EditorGUILayout.PropertyField(m_onSignalUndetected);

				EditorGUILayout.LabelField("Markup", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(m_onArrival);
				EditorGUILayout.PropertyField(m_onDeparture);
				EditorGUILayout.PropertyField(m_onReserved);
				EditorGUILayout.PropertyField(m_onCanceled);

				DrawNestedEvents();
			}
		}

		#endregion
	}
}