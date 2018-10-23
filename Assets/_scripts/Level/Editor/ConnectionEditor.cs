using UnityEditor;
using UnityEngine;

namespace Level
{
    [CustomEditor(typeof(Connection))]
    public class ConnectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Connection myTarget = (Connection)target;

            // Draw the script field
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Connection)target), typeof(Connection), false);
            GUI.enabled = true;

            myTarget.Traveling = (Connection.TravelingDirection)EditorGUILayout.EnumPopup("Traveling", myTarget.Traveling);
            if (myTarget.Traveling == Connection.TravelingDirection.Inbound)
            {

                var myIterator = serializedObject.FindProperty("Paths");
                PropertyField(myIterator);
            }
            else
            {
                var myIterator1 = serializedObject.FindProperty("ConnectsTo");
                EditorGUILayout.ObjectField(myIterator1);

                var myIterator2 = serializedObject.FindProperty("ConnectingEntity");
                EditorGUILayout.ObjectField(myIterator2);
            }
            // Apply changes to the serializedProperty - always do this at the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
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