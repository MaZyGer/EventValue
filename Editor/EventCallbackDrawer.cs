using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace Maz.Unity.Events.Editor
{

	[CustomPropertyDrawer(typeof(EventCallback), true)]
	public class EventCallbackDrawer : PropertyDrawer
	{
		private class PopupMenuItem
		{
			public GenericMenu.MenuFunction action;
			public string path;
			public GUIContent label;

			public PopupMenuItem(string path, string name, GenericMenu.MenuFunction action)
			{
				this.action = action;
				this.label = new GUIContent(name);
				this.path = path;
			}
		}

		float expanded_height = 0f;
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, false) + expanded_height + 12f;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			expanded_height = 0f;


			EditorGUI.BeginProperty(position, label, property);
			var tmpColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.black;
			GUI.Box(position, "");
			GUI.backgroundColor = tmpColor;
			
			SerializedProperty targetObjectProp = property.FindPropertyRelative("targetObject");


			//EditorGUI.PropertyField(position, property, true);

			position.y += 5f;
			position.height = 20f;
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(position, targetObjectProp, label, false);
			if (EditorGUI.EndChangeCheck())
			{
				
			}

			if (targetObjectProp.objectReferenceValue == null)
			{
				position.y += 22f;
				expanded_height += 22f;
				EditorGUI.LabelField(position, "No object assigned");
			} 
			else
			{
				DrawObjectMethods(position, property);
			}

			EditorGUI.EndProperty();
			
		}

		void DrawObjectMethods(Rect position, SerializedProperty property)
		{
			SerializedProperty targetObjectProp = property.FindPropertyRelative("targetObject");
			SerializedProperty selectedIndexProperty = property.FindPropertyRelative("selectedIndex");
			SerializedProperty methodNameProperty = property.FindPropertyRelative("methodName");

			var methodName = methodNameProperty.stringValue;

			//var isCachedProperty = property.FindPropertyRelative("isCached");
			var generics = fieldInfo.FieldType.GetGenericArguments();
			var returnType = generics.Last();

			var type = targetObjectProp.objectReferenceValue.GetType();
			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty).Where(m => m.ReturnType == returnType && m.GetParameters().Length == (generics.Length-1)).ToArray();
			var isCachedRef = typeof(EventCallback<>).GetField("isCached", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			var target = (EventCallback)fieldInfo.GetValue(property.serializedObject.targetObject);

			

			if (methods.Length <= 0)
				return;


			

			List<PopupMenuItem> items = new List<PopupMenuItem>();

			string[] methodNames = new string[methods.Length];

			for (int i = 0; i < methods.Length; i++)
			{
				
				var method = methods[i];
				var methodParams = method.GetParameters();
				var name = $"{method.ReturnType.Name} {method.Name}";
				methodNames[i] = name;

				items.Add(new PopupMenuItem("A", name, () => SetMethod(property, targetObjectProp.objectReferenceValue, method)));

			}

			position.y += 22f;
			expanded_height += 22f;

			var selectedMethod = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			if (selectedMethod != null)
				methodName = $"{selectedMethod.ReturnType.Name} {selectedMethod.Name}";

			var pos = EditorGUI.PrefixLabel(position, new GUIContent("Method"));
			if (EditorGUI.DropdownButton(pos, new GUIContent(methodName), FocusType.Keyboard))
			{
				GenericMenu menu = new GenericMenu();
				
				foreach (var i in items)
				{
					menu.AddItem(i.label, false, i.action);
				}
				menu.ShowAsContext();
			}

			if(selectedMethod != null)
			{
				var parameters = selectedMethod.GetParameters();

				if (parameters.Length > 0 && generics.Length > 1)
				{
					for (int i = 0; i < parameters.Length; i++)
					{
						var param = parameters[i];
						if(Argument.IsSupported(Argument.FromRealType(param.ParameterType)))
						{
							position.y += 21f;
							expanded_height += 21f;

							DrawArguments(position, property, param, i);
						}
					}
				}
				else
				{
					
				}
			}



			//if (EditorGUI.EndChangeCheck()) 
			//{
			//	selectedIndexProperty.intValue = selectedIndex;
			//	methodNameProperty.stringValue = methods[selectedIndexProperty.intValue].Name;
			//	isCachedRef.SetValue(target, false);
			//	SetMethod(property, targetObjectProp.objectReferenceValue, methods[selectedIndexProperty.intValue]);
			//}
		} 

		void DrawArguments(Rect position, SerializedProperty property, ParameterInfo param, int index)
		{
			var type = param.ParameterType;
			var paraName = $"{ObjectNames.NicifyVariableName(param.Name)} ({ObjectNames.NicifyVariableName(param.ParameterType.Name)})";
			SerializedProperty argumentsProperty = property.FindPropertyRelative("arguments");
			
			SerializedProperty elementProperty = argumentsProperty.GetArrayElementAtIndex(index);
			SerializedProperty parameterseProperty = elementProperty.FindPropertyRelative(Argument.FromRealType(param.ParameterType).ToString());

			if(parameterseProperty != null)
				EditorGUI.PropertyField(position, parameterseProperty, new GUIContent("    "  + paraName));
		}

		private void SetMethod(SerializedProperty property, UnityEngine.Object target, MethodInfo methodInfo)
		{
			SerializedProperty methodProp = property.FindPropertyRelative("methodName");
			methodProp.stringValue = methodInfo.Name;
			SerializedProperty argProp = property.FindPropertyRelative("arguments");
			ParameterInfo[] parameters = methodInfo.GetParameters();
			argProp.arraySize = parameters.Length;
			for (int i = 0; i < parameters.Length; i++)
			{
				argProp.FindPropertyRelative("Array.data[" + i + "].argumentType").enumValueIndex = (int)Argument.FromRealType(parameters[i].ParameterType);
				argProp.FindPropertyRelative("Array.data[" + i + "].argumentTypeName").stringValue = parameters[i].ParameterType.AssemblyQualifiedName;
			}
			property.FindPropertyRelative("isCached").boolValue = false;
			property.serializedObject.ApplyModifiedProperties();
			property.serializedObject.Update();
		}

		public static T GetValue<T>(SerializedProperty property) where T : class
		{
			object obj = property.serializedObject.targetObject;
			string path = property.propertyPath.Replace(".Array.data", "");
			string[] fieldStructure = path.Split('.');
			Regex rgx = new Regex(@"\[\d+\]");
			for (int i = 0; i < fieldStructure.Length; i++)
			{
				if (fieldStructure[i].Contains("["))
				{
					int index = System.Convert.ToInt32(new string(fieldStructure[i].Where(c => char.IsDigit(c)).ToArray()));
					obj = GetFieldValueWithIndex(rgx.Replace(fieldStructure[i], ""), obj, index);
				}
				else
				{
					obj = GetFieldValue(fieldStructure[i], obj);
				}
			}
			return (T)obj;
		}

		private static object GetFieldValue(string fieldName, object obj, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
		{
			FieldInfo field = obj.GetType().GetField(fieldName, bindings);
			if (field != null)
			{
				return field.GetValue(obj);
			}
			return default(object);
		}

		private static object GetFieldValueWithIndex(string fieldName, object obj, int index, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
		{
			FieldInfo field = obj.GetType().GetField(fieldName, bindings);
			if (field != null)
			{
				object list = field.GetValue(obj);
				if (list.GetType().IsArray)
				{
					return ((object[])list)[index];
				}
				else if (list is IEnumerable)
				{
					return ((IList)list)[index];
				}
			}
			return default(object);
		}
	}

}