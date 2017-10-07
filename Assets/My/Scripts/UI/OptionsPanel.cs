using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour {

	public Toggle musicCheck;
	public Toggle soundCheck;
	public Toggle qualityCheck;

	public void Init() {
		
		var colorChecks = GetComponentsInChildren<ColorCheck>();
		for (var i = 0; i < colorChecks.Length; i++) {
			colorChecks[i].Init();
		}
		var transparencyChecks = GetComponentsInChildren<TransparencyCheck>();
		for (var i = 0; i < transparencyChecks.Length; i++) {
			transparencyChecks[i].Init();
		}
		musicCheck.isOn = MainGameManager.Instance.gameConfig.isMusicOn;
		soundCheck.isOn = MainGameManager.Instance.gameConfig.isSoundOn;
	}

	void Start () {

		Init();

		qualityCheck.isOn = QualitySettings.GetQualityLevel() != 0;
		qualityCheck.onValueChanged.AddListener(MainGameManager.Instance.ChangeQuality);
	}

}
