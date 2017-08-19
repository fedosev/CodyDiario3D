using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor 
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        
        Grid grid = (Grid)target;
        if(GUILayout.Button("Generate Grid")) {
			for (var i = 0; i < 9; i++)
				grid.ClearGrid();
			grid.GenerateGrid();
        }
    }
}