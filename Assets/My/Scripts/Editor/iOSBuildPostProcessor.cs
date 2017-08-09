using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
public class iOSBuildPostProcessor : MonoBehaviour {

	#if UNITY_IOS
		[PostProcessBuild]
		static void OnPostprocessBuild(BuildTarget buildTarget, string path)
		{
			var plistPath = Path.Combine(path, "Info.plist");
			var plist = new PlistDocument();
			plist.ReadFromFile(plistPath);
	
			PlistElementDict rootDict = plist.root;
			rootDict.SetString("NSCameraUsageDescription", "La camera viene usata per la realtà aumentata");
	
			File.WriteAllText(plistPath, plist.WriteToString());
		}
	#endif
}
