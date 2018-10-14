using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Grid = Utility.Grid;

namespace Level
{
    [CustomEditor(typeof(Node))]
    public class NodeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var node = target as Node;

            var index = Grid.GetCellIndex(node.transform.position);
            EditorGUILayout.LabelField($"Cell Index\n{index}", new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter
            });
            EditorGUILayout.Space();
            DrawDefaultInspector();
        }
    }
}

