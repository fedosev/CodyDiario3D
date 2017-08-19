using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(BaseGameType), true)]
public class GameTypeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        BaseGameType gameType = (BaseGameType)target;

		var style = GUI.skin.GetStyle("Box");
		style.alignment = TextAnchor.MiddleLeft;

		GUILayout.Box(gameType.title);
		
		if (gameType.generalInfo != "") {
			GUILayout.Box(gameType.generalInfo, style);

			if(GUILayout.Button("Copy")) {
				GUIUtility.systemCopyBuffer = gameType.generalInfo;
			}
		}

    }
}