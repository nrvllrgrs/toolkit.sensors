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

		// Events
		protected SerializedProperty m_onFirstDetection;
		protected SerializedProperty m_onSignalDetected;
		protected SerializedProperty m_onSignalUndetected;
		protected SerializedProperty m_onLastUndetection;
		protected SerializedProperty m_onArrival;
		protected SerializedProperty m_onDeparture;
		protected SerializedProperty m_onReserved;
		protected SerializedProperty m_onCanceled;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_type = serializedObject.FindProperty(nameof(m_type));
			m_radius = serializedObject.FindProperty(nameof(m_radius));

			// Events
			m_onFirstDetection = serializedObject.FindProperty(nameof (m_onFirstDetection));
			m_onSignalDetected = serializedObject.FindProperty(nameof(m_onSignalDetected));
			m_onSignalUndetected = serializedObject.FindProperty(nameof(m_onSignalUndetected));
			m_onLastUndetection = serializedObject.FindProperty(nameof(m_onLastUndetection));
			m_onArrival = serializedObject.FindProperty(nameof(m_onArrival));
			m_onDeparture = serializedObject.FindProperty(nameof(m_onDeparture));
			m_onReserved = serializedObject.FindProperty(nameof(m_onReserved));
			m_onCanceled = serializedObject.FindProperty(nameof(m_onCanceled));
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_type);
			EditorGUILayout.PropertyField(m_radius);
		}

		protected override void DrawEvents()
		{
			if (EditorGUILayoutUtility.Foldout(m_onSignalDetected, "Events"))
			{
				EditorGUILayout.LabelField("Signal", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(m_onFirstDetection);
				EditorGUILayout.PropertyField(m_onSignalDetected);
				EditorGUILayout.PropertyField(m_onSignalUndetected);
				EditorGUILayout.PropertyField(m_onLastUndetection);

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