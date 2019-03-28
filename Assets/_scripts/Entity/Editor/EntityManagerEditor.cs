using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Level
{
    [CustomEditor(typeof(EntityManager), true)]
    [CanEditMultipleObjects]
    public class EntityManagerEditor : Editor
    {
        public float secs = 10f;
        public float startVal = 0f;
        public float progress = 0f;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            EntityManager myTarget = (EntityManager)target;

            EditorGUILayout.LabelField($"Stats");
            if (myTarget.Entities != null) EditorGUILayout.LabelField($"{myTarget.Entities.Length} Entities");
            if (myTarget.Connections != null) EditorGUILayout.LabelField($"{myTarget.Connections.Length} Connections");
            if (myTarget.Routes != null) EditorGUILayout.LabelField($"{myTarget.Routes.Length} Routes");
            if (myTarget.Routes != null) EditorGUILayout.LabelField($"{myTarget.Routes.Where(route => route != null).Sum(route => route.Terminals?.Length ?? 0)} Terminals");

            if (myTarget.Entities == null || myTarget.Connections == null || myTarget.Routes == null ||
                myTarget.Entities.Any(AreNull) || myTarget.Connections.Any(AreNull) || myTarget.Routes.Any(AreNull))
            {
                Debug.Log("Baking Entity Manager Automatically...");
                myTarget.Bake();
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Bake Level"))
            {
                Debug.Log("Baking Entity Manager...");
                myTarget.Bake();
                //NeutralVehicleManager.Instance.bakeNeutralPaths();
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