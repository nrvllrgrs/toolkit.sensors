using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using ToolkitEngine.Sensors;

namespace ToolkitEditor.Sensors
{
    [CustomEditor(typeof(BaseDetectionSensor), true)]
    public class BaseDetectionSensorEditor : BaseSensorEditor
    {
        #region Fields

        protected SerializedProperty m_detectionMode;
        protected SerializedProperty m_componentName;

        protected SerializedProperty m_detectionLayers;
        protected SerializedProperty m_queryTrigger;

        private static List<string> s_cachedComponentNames = null;

        #endregion

        #region Methods

        protected override void OnEnable()
        {
            base.OnEnable();
            m_detectionMode = serializedObject.FindProperty("m_detectionMode");
            m_componentName = serializedObject.FindProperty("m_componentName");
            m_detectionLayers = serializedObject.FindProperty("detectionLayers");
            m_queryTrigger = serializedObject.FindProperty("queryTrigger");
        }

        private void OnDisable()
        {
            s_cachedComponentNames = null;
        }

        protected override void OnCustomInspectorGUI()
        {
            base.OnCustomInspectorGUI();
            EditorGUILayout.Separator();
            OnDetectionInspectorGUI();
        }

        protected virtual void OnDetectionInspectorGUI()
        {
            DrawDetectionMode();
            EditorGUILayout.PropertyField(m_detectionLayers);
            EditorGUILayout.PropertyField(m_queryTrigger);
        }

        protected void DrawDetectionMode()
        {
            EditorGUILayout.PropertyField(m_detectionMode);
            if ((DetectionMode)m_detectionMode.intValue == DetectionMode.Custom)
            {
                if (s_cachedComponentNames == null)
                {
                    s_cachedComponentNames = System.AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes())
                        .Where(type => type.IsClass && type.IsSubclassOf(typeof(Component)))
                        .Select(x => x.Name)
                        .OrderBy(x => x).ToList();
                }

                int index = 0;
                if (!string.IsNullOrWhiteSpace(m_componentName.stringValue))
                {
                    index = s_cachedComponentNames.IndexOf(m_componentName.stringValue);
                }

                if (index >= 0)
                {
                    index = EditorGUILayout.Popup(m_componentName.displayName, index, s_cachedComponentNames.ToArray());
                    m_componentName.stringValue = s_cachedComponentNames[index];
                }
                else
                {
                    EditorGUILayout.HelpBox(string.Format("Component type \"{0}\" does not exist!", m_componentName.stringValue), MessageType.Error);
                    EditorGUILayout.PropertyField(m_componentName);
                }
            }
        }

        #endregion
    }
}