using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorsPanel : MonoBehaviour {

	public ColorCheck[] colorChecks;
	public ARFormOptions.GridColors gridColors;

	public void Init () {

		for (var i = 0; i < colorChecks.Length; i++) {
			colorChecks[i].Setup(i, gridColors.items[i].color);
		}
	}
	
}
