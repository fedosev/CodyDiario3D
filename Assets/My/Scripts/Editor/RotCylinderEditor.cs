using UnityEngine;
using System.Collections;
using UnityEditor;
using TMPro;

[CustomEditor(typeof(RotCylinder))]
public class RotCylinderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        RotCylinder rotCylinder = (RotCylinder)target;
		
        if(GUILayout.Button("Generate Chars")) {
			var n = 26 + (rotCylinder.withSpace ? 1 : 0);
			string charStr;
			GameObject charGameObj;
			for (var i = 0; i < n; i++) {
				charGameObj = Instantiate(/*config.RotCylinderCharPrefab*/rotCylinder.charPrefab, Vector3.zero, Quaternion.Euler(-(360f * i / n), 0, 0));
				charGameObj.transform.parent = rotCylinder.transform;
				if (i < 26) {
					charStr = ((char)(65 + i)).ToString();
				} else {
					charStr = " ";
				}
				charGameObj.GetComponentInChildren<TextMeshPro>().text = charStr;
				charGameObj.name = "Char(" + charStr + ")";

			}
        }
    }
}