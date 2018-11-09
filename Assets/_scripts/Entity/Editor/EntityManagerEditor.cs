using UnityEditor;
using UnityEngine;

namespace Level
{
    [CustomEditor(typeof(EntityManager), true)]
    [CanEditMultipleObjects]
    public class EntityManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            EntityManager myTarget = (EntityManager)target;

            EditorGUILayout.LabelField($"Stats");
            EditorGUILayout.LabelField($"{myTarget.Entities.Length} Entities");
            EditorGUILayout.LabelField($"{myTarget.Connections.Length} Connections");

            EditorGUILayout.Space();
            if (GUILayout.Button("Bake Level"))
            {
                myTarget.Bake();
            }
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