using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(AllGameTypes))]
public class AllGameTypesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        AllGameTypes allGameTypes = (AllGameTypes)target;

        if (allGameTypes.items != null) {
            GUILayout.Box(allGameTypes.items.Count + " items loaded");
        }
        if(GUILayout.Button("Reload all")) {
            //allGameTypes.items = new Dictionary<string, BaseGameType>();
            allGameTypes.items.Clear();
            var guids = AssetDatabase.FindAssets("", new[] { "Assets/My/Config/GameTypes" });
            foreach (string guid in guids) {
                var asset = (BaseGameType)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(BaseGameType));
                //allGameTypes.items.Add(asset.name, asset);
                allGameTypes.items.Add(asset);
			    Debug.Log("Load " + asset.ToString() + " - " + asset.name);
                //AssetDatabase.SaveAssets();
            }
        }
        if(GUILayout.Button("Save")) {
            EditorUtility.SetDirty(allGameTypes);
            AssetDatabase.SaveAssets();
            //allGameTypes.Save();
        }        
    }
}