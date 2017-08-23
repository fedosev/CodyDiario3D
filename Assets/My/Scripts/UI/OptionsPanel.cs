using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsPanel : MonoBehaviour {


	public void Init() {
		
		var colorChecks = GetComponentsInChildren<ColorCheck>();
		for (var i = 0; i < colorChecks.Length; i++) {
			colorChecks[i].Init();
		}
		var transparencyChecks = GetComponentsInChildren<TransparencyCheck>();
		for (var i = 0; i < transparencyChecks.Length; i++) {
			transparencyChecks[i].Init();
		}
	}

	void Start () {

		Init();
	}

}
