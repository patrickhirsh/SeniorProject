using UnityEditor;
using UnityEngine;

namespace Level
{
    [CustomEditor(typeof(Entity), true)]
    [CanEditMultipleObjects]
    public class EntityEditor : Editor
    {
        private bool _showNeighbors;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            Entity myTarget = (Entity)target;

            EditorGUILayout.LabelField($"Stats");
            EditorGUILayout.LabelField($"{myTarget.Connections.Length} Connections");
            EditorGUILayout.LabelField($"{myTarget.Paths.Length} Paths");
            EditorGUILayout.LabelField($"{myTarget.Nodes.Length} Nodes");
            _showNeighbors = EditorGUILayout.Foldout(_showNeighbors, $"{myTarget.NeighborEntities.Length} Neighbors");
            if (_showNeighbors)
            {
                for (int i = 0; i < myTarget.NeighborEntities.Length; i++)
                {
                    EditorGUILayout.ObjectField($"Neighbor {i}", myTarget.NeighborEntities[i], typeof(Entity));
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Bake Entity For Prefab"))
            {
                myTarget.BakePrefab();
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