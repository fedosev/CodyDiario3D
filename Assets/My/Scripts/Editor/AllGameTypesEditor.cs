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

        if (allGameTypes.testItems != null) {
            GUILayout.Box(allGameTypes.testItems.Count + " items loaded");
        }
        if(GUILayout.Button("Reload all")) {
            //allGameTypes.items = new Dictionary<string, BaseGameType>();
            allGameTypes.testItems.Clear();
            var guids = AssetDatabase.FindAssets("", new[] { "Assets/My/Config/TestGameTypes" });
            foreach (string guid in guids) {
                var asset = (BaseGameType)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(BaseGameType));
                //allGameTypes.items.Add(asset.name, asset);
                allGameTypes.testItems.Add(asset);
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