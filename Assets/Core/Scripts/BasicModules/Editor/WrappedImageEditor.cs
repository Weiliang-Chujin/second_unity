using Core.Scripts.BasicModules.Components;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Core.Scripts.BasicModules.Editor
{
    [CustomEditor(typeof(WrappedImage))]
    public class WrappedImageEditor : ImageEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SerializedProperty serializedProperty = serializedObject.FindProperty("sprites");
            EditorGUILayout.PropertyField(serializedProperty);

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}