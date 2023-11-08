using UnityEngine;
using UnityEditor;
using ToolkitEngine.Sensors;

namespace ToolkitEditor.Sensors
{
    [CustomEditor(typeof(MarkupSensor))]
    public class MarkupSensorEditor : BaseSensorEditor
	{
		#region Fields

		protected SerializedProperty m_pulse;
		protected SerializedProperty m_pulseMode;
		protected SerializedProperty m_radius;

		// Events
		protected SerializedProperty m_onPulsed;

		#endregion

		#region Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			m_pulse = serializedObject.FindProperty(nameof(m_pulse));
			m_pulseMode = m_pulse.FindPropertyRelative("pulseMode");
			m_radius = serializedObject.FindProperty("radius");

			// Events
			m_onPulsed = serializedObject.FindProperty(nameof(m_onPulsed));
		}

		protected override void OnCustomInspectorGUI()
		{
			EditorGUILayout.PropertyField(m_pulseMode);
			base.OnCustomInspectorGUI();

			EditorGUILayout.Separator();

			EditorGUILayout.PropertyField(m_radius);
		}

		protected override void OnSignalsInspectorGUI()
		{
			if (target is IPulseableSensor sensor && sensor.pulseMode == PulseMode.Manual)
			{
				EditorGUILayout.Separator();
				if (GUILayout.Button("Pulse"))
				{
					sensor.Pulse();
				}
			}

			base.OnSignalsInspectorGUI();
		}

		protected override void OnEventInspectorGUI()
		{
			base.OnEventInspectorGUI();
			EditorGUILayout.PropertyField(m_onPulsed);
		}

		#endregion
	}
}