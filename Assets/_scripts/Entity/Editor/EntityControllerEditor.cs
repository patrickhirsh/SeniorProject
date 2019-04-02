using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RideShareLevel
{
    [CustomEditor(typeof(EntityController), true)]
    [CanEditMultipleObjects]
    public class EntityControllerEditor : Editor
    {
        public float secs = 10f;
        public float startVal = 0f;
        public float progress = 0f;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            EntityController myTarget = (EntityController)target;

            EditorGUILayout.LabelField($"Stats");
            if (myTarget.Entities != null) EditorGUILayout.LabelField($"{myTarget.Entities.Length} Entities");
            if (myTarget.Connections != null) EditorGUILayout.LabelField($"{myTarget.Connections.Length} Connections");
            if (myTarget.Buildings != null) EditorGUILayout.LabelField($"{myTarget.Buildings.Length} Buildings");
            if (myTarget.Routes != null) EditorGUILayout.LabelField($"{myTarget.Routes.Length} Routes");
            if (myTarget.Routes != null) EditorGUILayout.LabelField($"{myTarget.Routes.Where(route => route != null).Sum(route => route.Terminals?.Length ?? 0)} Terminals");

            if (myTarget.Entities == null || myTarget.Connections == null || myTarget.Routes == null ||
                myTarget.Entities.Any(AreNull) || myTarget.Connections.Any(AreNull) || myTarget.Routes.Any(AreNull))
            {
                Debug.Log("Detected changes in Entity Manager for level. Baking Entity Manager Automatically...");
                myTarget.Bake();
            }
        }

        private bool AreNull(Object obj)
        {
            return obj == null;
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