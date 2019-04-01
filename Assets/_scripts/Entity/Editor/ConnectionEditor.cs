using UnityEditor;
using UnityEngine;

namespace RideShareLevel
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
            EditorGUILayout.LabelField($"{myTarget.Paths.Count} VehiclePaths");

            EditorGUILayout.ObjectField("Connects To", myTarget.GetConnectsTo, typeof(Connection), true);
            EditorGUILayout.ObjectField("Parent", myTarget.ParentRoute, typeof(Entity), true);
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