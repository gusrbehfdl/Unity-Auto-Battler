using AutoBattleFramework.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AutoBattleFramework.EditorScripts
{
    /// <summary>
    /// A variable with the ReadOnly tag cannot be modified in the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property,
                                                GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position,
                                   SerializedProperty property,
                                   GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}