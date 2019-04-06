using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Grid = Utility.Grid;

namespace RideShareLevel
{
    [CustomEditor(typeof(Entity))]
    public class NodeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var node = target as Entity;

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

