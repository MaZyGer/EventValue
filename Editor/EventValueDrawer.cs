using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Maz.Unity.Events.Editor
{

	[CustomPropertyDrawer(typeof(EventValue<>))]
	public class EventValueDrawer : PropertyDrawer
	{
		float EXPAND_HEIGHT = 0f;
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float neededHeight = EditorGUI.GetPropertyHeight(property, label, true);
			return neededHeight + EXPAND_HEIGHT;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EXPAND_HEIGHT = 0f;
			float neededHeight = 0f;

			position.height = 18f;
			label = EditorGUI.BeginProperty(position, label, property);
			var valueProp = property.FindPropertyRelative("value");

			bool isExpanded = false;

			if (!valueProp.hasChildren)
			{
				neededHeight = EditorGUI.GetPropertyHeight(valueProp, true);
				position.height = neededHeight;
				//EditorGUI.PropertyField(position, property, label, false);

				var widthOld = position.width;
				position.width = GUI.skin.label.CalcSize(label).x;
				property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
				position.width = widthOld;

				neededHeight = EditorGUI.GetPropertyHeight(valueProp, true);
				position.height = neededHeight + 2f;
				EditorGUI.PropertyField(position, valueProp, new GUIContent(" "), true);

				isExpanded = property.isExpanded;

			} else {

				neededHeight = EditorGUI.GetPropertyHeight(valueProp, true);
				position.height = neededHeight;
				EditorGUI.PropertyField(position, valueProp, label, true);
				isExpanded = property.isExpanded = valueProp.isExpanded;
				position.y += 4f;
			}





			EditorGUI.BeginChangeCheck();
			if (isExpanded)
			{

				var onChangedEventProperty = property.FindPropertyRelative("OnChangedEvent");
				position.y += position.height;
				position.height = EditorGUI.GetPropertyHeight(onChangedEventProperty, true);
				EditorGUI.PropertyField(position, onChangedEventProperty, label);
			}

			// Only assign the value back if it was actually changed by the user.
			// Otherwise a single value will be assigned to all objects when multi-object editing,
			// even when the user didn't touch the control.
			if (EditorGUI.EndChangeCheck())
			{
				//fieldInfo.GetValue(valueProp.serializedObject.targetObject);
				//Debug.Log(valueProp);
				//var newIndex = valueProp.enumValueIndex;
				//if(!index.Equals(newIndex))
				//{
				//	var obj = (IEventValue)fieldInfo.GetValue(valueProp.serializedObject.targetObject);
				//	//obj.OnChange();
				//	//obj.Raise();
				//}
			}
			EditorGUI.EndProperty();
		}
	} 
}
