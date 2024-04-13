using Etienne.Animator2D;
using UnityEditor;
using UnityEngine;

namespace EtienneEditor.Animator2D
{
    [CustomPropertyDrawer(typeof(Frame))]
    internal class FrameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float w = EditorGUIUtility.labelWidth;

            SerializedProperty sprite = property.FindPropertyRelative("sprite");
            label.text = "";
            position.width -= 72;
            EditorGUI.ObjectField(position, sprite, new GUIContent(""));
            position.x += position.width + 2;
            position.width = 60;
            SerializedProperty duration = property.FindPropertyRelative("duration");
            EditorGUIUtility.labelWidth = 20;
            duration.floatValue = Mathf.Max(0f, EditorGUI.FloatField(position, " ", duration.floatValue));
            position.x += position.width + 2;
            EditorGUI.LabelField(position, new GUIContent("F"), EditorStyles.boldLabel);

            EditorGUIUtility.labelWidth = w;
        }
    }
}
