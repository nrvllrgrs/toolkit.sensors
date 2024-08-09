using UnityEditor;
using ToolkitEngine.Sensors;

namespace ToolkitEditor.Sensors
{
	[CustomEditor(typeof(TriggerSensor), true)]
	public class TriggerSensorEditor : BaseColliderSensorEditor
    {
		#region Fields

		protected SerializedProperty m_onDetectedTail;
		protected SerializedProperty m_onDetectedTip;
		protected SerializedProperty m_onUndetectedTail;
		protected SerializedProperty m_onUndetectedTip;
		protected SerializedProperty m_onUndetectedTailToTip;
		protected SerializedProperty m_onUndetectedTipToTail;

		#endregion

		#region Methods

		protected override void OnEnable()
		{
			base.OnEnable();
			m_onDetectedTail = serializedObject.FindProperty(nameof(m_onDetectedTail));
			m_onDetectedTip = serializedObject.FindProperty(nameof(m_onDetectedTip));
			m_onUndetectedTail = serializedObject.FindProperty(nameof(m_onUndetectedTail));
			m_onUndetectedTip = serializedObject.FindProperty(nameof(m_onUndetectedTip));
			m_onUndetectedTailToTip = serializedObject.FindProperty(nameof(m_onUndetectedTailToTip));
			m_onUndetectedTipToTail = serializedObject.FindProperty(nameof(m_onUndetectedTipToTail));
		}

		protected override void OnEventInspectorGUI()
		{
			base.OnEventInspectorGUI();

			EditorGUILayout.LabelField("Enter Flow", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(m_onDetectedTail);
			EditorGUILayout.PropertyField(m_onDetectedTip);

			EditorGUILayout.LabelField("Exit Flow", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(m_onUndetectedTail);
			EditorGUILayout.PropertyField(m_onUndetectedTip);
			EditorGUILayout.PropertyField(m_onUndetectedTailToTip);
			EditorGUILayout.PropertyField(m_onUndetectedTipToTail);
		}

		#endregion
	}
}