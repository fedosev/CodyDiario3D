using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ColorsPanel))]
public class ColorsPanelEditor : Editor 
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        
        ColorsPanel colorsPanel = (ColorsPanel)target;
        if(GUILayout.Button("Update colors")) {
			colorsPanel.Init();
        }
    }
}