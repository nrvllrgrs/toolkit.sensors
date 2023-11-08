using UnityEditor;
using ToolkitEngine.Sensors;
using UnityEngine;

namespace ToolkitEditor.Sensors
{
	[CustomEditor(typeof(Perception)), CanEditMultipleObjects]
    public class PerceptionEditor : BaseToolkitEditor
    {
		#region Fields

		protected Perception m_perception;

		protected SerializedProperty m_senses;
		protected SerializedProperty m_drainDelay;
		protected SerializedProperty m_drainRate;
		protected SerializedProperty m_targetElector;

		protected SerializedProperty m_onFirstStimulusSensed;
		protected SerializedProperty m_onStimulusSensed;
		protected SerializedProperty m_onStimulusUnsensed;
		protected SerializedProperty m_onLastStimulusUnsensed;

		protected SerializedProperty m_onStimulusConfidenceChanged;
		protected SerializedProperty m_onStimulusKnown;
		protected SerializedProperty m_onStimulusUnknown;
		protected SerializedProperty m_onTargetChanged;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_perception = target as Perception;

			m_senses = serializedObject.FindProperty(nameof(m_senses));
			m_drainDelay = serializedObject.FindProperty(nameof(m_drainDelay));
			m_drainRate = serializedObject.FindProperty(nameof(m_drainRate));
			m_targetElector = serializedObject.FindProperty(nameof(m_targetElector));

			m_onFirstStimulusSensed = serializedObject.FindProperty(nameof(m_onFirstStimulusSensed));
			m_onStimulusSensed = serializedObject.FindProperty(nameof(m_onStimulusSensed));
			m_onStimulusUnsensed = serializedObject.FindProperty(nameof(m_onStimulusUnsensed));
			m_onLastStimulusUnsensed = serializedObject.FindProperty(nameof(m_onLastStimulusUnsensed));
			m_onStimulusConfidenceChanged = serializedObject.FindProperty(nameof(m_onStimulusConfidenceChanged));
			m_onStimulusKnown = serializedObject.FindProperty(nameof(m_onStimulusKnown));
			m_onStimulusUnknown = serializedObject.FindProperty(nameof(m_onStimulusUnknown));
			m_onTargetChanged = serializedObject.FindProperty(nameof(m_onTargetChanged));

			if (Application.isPlaying)
			{
				m_perception.onStimulusConfidenceChanged.AddListener(StimulusConfidenceChanged);
			}
		}

		private void OnDisable()
		{
			if (Application.isPlaying)
			{
				m_perception.onStimulusConfidenceChanged.RemoveListener(StimulusConfidenceChanged);
			}
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_senses);

			if (EditorGUILayoutUtility.Foldout(m_drainDelay, "Drain"))
			{
				++EditorGUI.indentLevel;
				EditorGUILayout.PropertyField(m_drainDelay, new GUIContent("Delay"));
				EditorGUILayout.PropertyField(m_drainRate, new GUIContent("Rate"));
				--EditorGUI.indentLevel;
			}

			EditorGUILayout.PropertyField(m_targetElector);

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.ObjectField("Target", m_perception.target, typeof(GameObject), false);
			EditorGUILayout.FloatField("Confidence", m_perception.confidence);
			EditorGUI.EndDisabledGroup();
		}

		protected override void DrawEvents()
		{
			if (EditorGUILayoutUtility.Foldout(m_onTargetChanged, "Events"))
			{
				EditorGUILayout.LabelField("Sense", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(m_onFirstStimulusSensed);
				EditorGUILayout.PropertyField(m_onStimulusSensed);
				EditorGUILayout.PropertyField(m_onStimulusUnsensed);
				EditorGUILayout.PropertyField(m_onLastStimulusUnsensed);

				EditorGUILayout.LabelField("Stimulus", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(m_onStimulusConfidenceChanged);
				EditorGUILayout.PropertyField(m_onStimulusKnown);
				EditorGUILayout.PropertyField(m_onStimulusUnknown);
				EditorGUILayout.PropertyField(m_onTargetChanged);

				DrawNestedEvents();
			}
		}

		private void StimulusConfidenceChanged(PerceptionEventArgs e)
		{
			if (Equals(e.perception.target, e.stimulus.gameObject))
			{
				Repaint();
			}
		}

		#endregion
	}
}