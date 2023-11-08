using UnityEditor;
using System.Collections.Generic;

namespace ToolkitEngine.Sensors
{
    [CustomEditor(typeof(SignalType))]
    public class SignaTypeEditor : Editor
    {
        #region Fields

        protected SignalType m_signalType;

        protected SerializedProperty m_parent;

        #endregion

        #region Methods

        private void OnEnable()
        {
            m_signalType = target as SignalType;

            m_parent = serializedObject.FindProperty(nameof(m_parent));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //EditorGUILayout.PropertyField(m_id);

            EditorGUI.BeginChangeCheck();

            var lastParent = m_parent.objectReferenceValue;
            EditorGUILayout.PropertyField(m_parent);

            if (EditorGUI.EndChangeCheck())
            {
                if (m_parent.objectReferenceValue != null)
                {
                    var parentSignalType = m_parent.objectReferenceValue as SignalType;
                    if (!IsValidParent(parentSignalType))
                    {
                        EditorUtility.DisplayDialog("Error", string.Format("{0} creates an invalid parental relationship!", parentSignalType.name), "OK");
                        m_parent.objectReferenceValue = lastParent;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private bool IsValidParent(SignalType parent)
        {
            if (parent == null)
                return true;

            HashSet<SignalType> set = new();
            set.Add(m_signalType);

            var test = parent;
            while (test != null)
            {
                if (set.Contains(test))
                    return false;

                set.Add(test);
                test = test.Parent;
            }

            return true;
        }

        #endregion
    }
}