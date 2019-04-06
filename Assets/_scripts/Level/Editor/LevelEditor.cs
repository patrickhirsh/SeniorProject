using RideShareLevel;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level), true)]
[CanEditMultipleObjects]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        var myTarget = (Level)target;


        EditorGUILayout.Space();
        if (GUILayout.Button("Bake Level"))
        {
            Debug.Log("Baking Level...");
            myTarget.Bake();
        }
    }
}
