using UnityEngine;
using UnityEditor;
using System.IO;
 
 [InitializeOnLoad]
public class PreloadSigningAlias {
  
	static PreloadSigningAlias () {
		PlayerSettings.Android.keystorePass = "NeuNeunet201_7";
		PlayerSettings.Android.keyaliasName = "codydiario3dkey";
		PlayerSettings.Android.keyaliasPass = "NeuNeunet201_7";
	}
}