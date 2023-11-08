using UnityEngine;
using UnityEditor;
using ToolkitEngine.Sensors;

namespace ToolkitEditor.Sensors
{
    [CustomEditor(typeof(BaseSensor), true)]
    public class BaseSensorEditor : Editor
    {
        #region Fields

        protected SerializedProperty m_strengthCalculator;
        protected SerializedProperty m_filters;

        protected SerializedProperty m_set;

        // Events
        protected SerializedProperty m_onFirstDetection;
        protected SerializedProperty m_onSignalDetected;
        protected SerializedProperty m_onSignalUndetected;
        protected SerializedProperty m_onLastUndetection;

        #endregion

        #region Methods

        protected virtual void OnEnable()
        {
            m_strengthCalculator = serializedObject.FindProperty(nameof(m_strengthCalculator));
            m_filters = serializedObject.FindProperty(nameof(m_filters));
            m_set = serializedObject.FindProperty(nameof(m_set));

            // Events
            m_onFirstDetection = serializedObject.FindProperty(nameof(m_onFirstDetection));
            m_onSignalDetected = serializedObject.FindProperty(nameof(m_onSignalDetected));
            m_onSignalUndetected = serializedObject.FindProperty(nameof(m_onSignalUndetected));
            m_onLastUndetection = serializedObject.FindProperty(nameof(m_onLastUndetection));

            if (Application.isPlaying)
            {
				var sensor = target as BaseSensor;
				sensor.onSignalDetected.AddListener(SignalDirty);
                sensor.onSignalUndetected.AddListener(SignalDirty);
            }
        }

        protected virtual void OnDisable()
        {
			if (Application.isPlaying)
			{
                var sensor = target as BaseSensor;
				sensor.onSignalDetected.RemoveListener(SignalDirty);
				sensor.onSignalUndetected.RemoveListener(SignalDirty);
			}
		}

        private void SignalDirty(SensorEventArgs e)
        {
            Repaint();
        }


		public override void OnInspectorGUI()
        {
            serializedObject.Update();

			using (new EditorGUI.DisabledScope(true))
			{
				if (target is MonoBehaviour behaviour)
				{
					EditorGUILayout.ObjectField(EditorGUIUtility.TrTempContent("Script"), MonoScript.FromMonoBehaviour(behaviour), typeof(MonoBehaviour), false);
				}
			}
			OnCustomInspectorGUI();

            EditorGUILayout.PropertyField(m_filters);

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_set);

            if (EditorApplication.isPlaying)
            {
                OnSignalsInspectorGUI();
            }

            EditorGUILayout.Separator();
            if (EditorGUILayoutUtility.Foldout(m_onFirstDetection, "Events"))
            {
                OnEventInspectorGUI();
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnCustomInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_strengthCalculator);
        }

        protected virtual void OnSignalsInspectorGUI()
        {
            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.LabelField("Signals", EditorStyles.boldLabel);

            var sensor = target as BaseSensor;
            foreach (var signal in sensor.signals)
            {
                if (signal?.detected.IsNull() ?? true)
                    continue;

                EditorGUILayout.LabelField(signal.detected.name, string.Format("Str: {0}", signal.strength));
            }

            GUILayout.EndVertical();
        }

        protected virtual void OnEventInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_onFirstDetection);
            EditorGUILayout.PropertyField(m_onSignalDetected);
            EditorGUILayout.PropertyField(m_onSignalUndetected);
            EditorGUILayout.PropertyField(m_onLastUndetection);
        }

        #endregion
    }
}