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

		private static List<string> s_cachedDisplayNames = null;
		private static List<string> s_cachedTypeNames = null;

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

        protected override void OnDisable()
        {
            s_cachedDisplayNames = null;
			s_cachedTypeNames = null;
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
                if (s_cachedDisplayNames == null)
                {
                    s_cachedDisplayNames = new();
                    s_cachedTypeNames = new();

					foreach (var p in from assembly in System.AppDomain.CurrentDomain.GetAssemblies()
									  let assemblyName = assembly.GetName().Name
									  from type in assembly.GetTypes()
									  where type.IsClass && type.IsSubclassOf(typeof(Component))
									  orderby assemblyName, type.Name
                                      select new { displayName = $"{assemblyName}/{type.Name}", name = type.Name })
                    {
                        s_cachedDisplayNames.Add(p.displayName);
                        s_cachedTypeNames.Add(p.name);
                    }
				}

                int index = 0;
                if (!string.IsNullOrWhiteSpace(m_componentName.stringValue))
                {
                    index = s_cachedTypeNames.IndexOf(m_componentName.stringValue);
                }

                if (index >= 0)
                {
                    index = EditorGUILayout.Popup(m_componentName.displayName, index, s_cachedDisplayNames.ToArray());
                    m_componentName.stringValue = s_cachedTypeNames[index];
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