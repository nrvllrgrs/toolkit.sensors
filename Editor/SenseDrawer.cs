using UnityEngine;
using UnityEditor;
using ToolkitEngine;
using ToolkitEngine.Sensors;

namespace ToolkitEditor.Sensors
{
    [CustomPropertyDrawer(typeof(Perception.Sense))]
    public class SenseDrawer : PropertyDrawer
    {
		#region Fields

		private const float k_enabledWidth = 20f;

		#endregion

		#region Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
			var nameProp = property.FindPropertyRelative("m_name");
			label.tooltip = property.tooltip;
			label.text = nameProp.stringValue;

			var rect = new Rect(position);
			var width = rect.width - 2f;
			rect.y += 1f;
			rect.height = EditorGUIUtility.singleLineHeight;

			// Draw the enabled property
			rect.width = k_enabledWidth;
			rect.x += 4f;

			var enabledProp = property.FindPropertyRelative("m_enabled");
			if (enabledProp != null)
			{
				EditorGUIRectLayout.PropertyField(ref rect, enabledProp, GUIContent.none);
			}
			rect.x += rect.width + 10f;
			rect.y = position.y;
			rect.width = width - k_enabledWidth - 10f;

			if (!EditorGUIRectLayout.Foldout(ref rect, nameProp, label))
                return;

			position.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

			++EditorGUI.indentLevel;
            EditorGUIRectLayout.PropertyField(ref position, nameProp);
			EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_priority"));

			var sensorProp = property.FindPropertyRelative("m_sensor");
			EditorGUIRectLayout.PropertyField(ref position, sensorProp);

			if (sensorProp.objectReferenceValue is IPulseableSensor)
			{
				++EditorGUI.indentLevel;
				EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_autoPulse"));
				--EditorGUI.indentLevel;
			}

			EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_strengthThreshold"));
			EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_confidence"));
			EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_useConfidence"));
			EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_useStrength"));
			--EditorGUI.indentLevel;
		}

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
			float height = EditorGUIUtility.singleLineHeight
				+ EditorGUIUtility.standardVerticalSpacing;

			var nameProp = property.FindPropertyRelative("m_name");
			if (nameProp.isExpanded)
			{
				var sensorProp = property.FindPropertyRelative("m_sensor");
				height += EditorGUI.GetPropertyHeight(sensorProp)
					+ EditorGUI.GetPropertyHeight(sensorProp)
					+ (EditorGUIUtility.standardVerticalSpacing * 2);

				if (sensorProp.objectReferenceValue is IPulseableSensor)
				{
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_autoPulse"))
						+ EditorGUIUtility.standardVerticalSpacing;
				}

				height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_priority"))
					+ EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_strengthThreshold"))
					+ EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_confidence"))
					+ EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_useConfidence"))
					+ EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_useStrength"))
					+ (EditorGUIUtility.standardVerticalSpacing * 5);
			}

			return height;
		}

		#endregion
	}
}