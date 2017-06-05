using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateGridColors {

    [MenuItem("Assets/Create/Gird Colors")]
    public static ARFormOptions.GridColors Create() {
        ARFormOptions.GridColors asset = ScriptableObject.CreateInstance<ARFormOptions.GridColors>();

        AssetDatabase.CreateAsset(asset, "Assets/GridColors.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}
