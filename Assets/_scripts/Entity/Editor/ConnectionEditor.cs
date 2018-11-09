using UnityEditor;
using UnityEngine;

namespace Level
{
    [CustomEditor(typeof(Connection))]
    [CanEditMultipleObjects]
    public class ConnectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            Connection myTarget = (Connection)target;

            EditorGUILayout.LabelField($"Stats");
            EditorGUILayout.LabelField($"{myTarget.Paths.Count} Paths");

            EditorGUILayout.ObjectField("Connects To", myTarget.ConnectsTo, typeof(Connection));
            EditorGUILayout.ObjectField("Parent Entity", myTarget.ParentEntity, typeof(Entity));
        }

        private static void PropertyField(SerializedProperty myIterator)
        {
            while (true)
            {
                var myRect = GUILayoutUtility.GetRect(0f, 16f);
                var showChildren = EditorGUI.PropertyField(myRect, myIterator);
                if (!myIterator.NextVisible(showChildren)) break;
            }
        }
    }
}