using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RotCylinder))]
public class RotCylinderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        RotCylinder rotCylinder = (RotCylinder)target;
		
        if(GUILayout.Button("Generate Chars")) {
			rotCylinder.GenerateChars();
        }
    }
}