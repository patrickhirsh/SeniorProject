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
            if (myTarget.Entities != null) EditorGUILayout.LabelField($"{myTarget.Entities.Length} Entities");
            if (myTarget.Connections != null) EditorGUILayout.LabelField($"{myTarget.Connections.Length} Connections");

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