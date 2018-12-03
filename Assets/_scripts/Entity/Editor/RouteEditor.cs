using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Level
{
    [CustomEditor(typeof(Route), true)]
    [CanEditMultipleObjects]
    public class RouteEditor : Editor
    {
        private bool _showNeighbors;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            Route myTarget = (Route)target;

            EditorGUILayout.LabelField($"Stats");
            EditorGUILayout.LabelField($"{myTarget.Nodes.Count} Nodes");
            EditorGUILayout.LabelField($"{myTarget.Connections.Length} Connections");
            EditorGUILayout.LabelField($"{myTarget.VehiclePaths.Length} Vehicle Paths");
            EditorGUILayout.LabelField($"{myTarget.Connections.SelectMany(connection => connection.Terminals).Count()} Terminals");
            _showNeighbors = EditorGUILayout.Foldout(_showNeighbors, $"{myTarget.NeighborRoutes.Length} Neighbors");
            if (_showNeighbors)
            {
                for (int i = 0; i < myTarget.NeighborRoutes.Length; i++)
                {
                    EditorGUILayout.ObjectField($"Neighbor {i}", myTarget.NeighborRoutes[i], typeof(Entity), true);
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