using RideShareLevel;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Snap))]
public class SnapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Snap snap = (Snap)target;

        if (GUILayout.Button("Snap Rotation"))
        {
            snap.SnapRotation();
        }
    }
}