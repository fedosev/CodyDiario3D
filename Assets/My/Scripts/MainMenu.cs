using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour {

	public bool isHidden = false;
	public TMP_Dropdown gameTypeSelector;

	MainGameManager gameManager;


	void Awake() {
		gameManager = FindObjectOfType<MainGameManager>();
	}

	// Use this for initialization
	void Start () {
		List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
		TMP_Dropdown.OptionData optData;
		foreach (var item in gameManager.allGameTypes.items) {
			optData = new TMP_Dropdown.OptionData(item.name);
			options.Add(optData);
		}
		gameTypeSelector.options = options;
		gameTypeSelector.onValueChanged.AddListener(gameManager.LoadGameType);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
