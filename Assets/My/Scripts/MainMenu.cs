using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using TMPro;

public class MainMenu : MonoBehaviour {

	public bool isHidden = false;
	public Dropdown gameTypeSelector;

	MainGameManager gameManager;


	void Awake() {
		gameManager = MainGameManager.Instance;
	}

	public void SetupGameTyoeSelector() {
		List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
		Dropdown.OptionData optData;
		foreach (var item in gameManager.allGameTypes.items) {
			optData = new Dropdown.OptionData(item.name);
			options.Add(optData);
		}
		gameTypeSelector.options = options;
		//gameTypeSelector.ClearOptions();
		//gameTypeSelector.AddOptions(options);

		gameTypeSelector.onValueChanged.AddListener(gameManager.LoadGameType);
	}

	// Use this for initialization
	void Start () {

		SetupGameTyoeSelector();

		var inputField = GameObject.Find("InputFieldiOS").GetComponent<InputField>();
		#if UNITY_IOS
			gameTypeSelector.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150f);
			inputField.onEndEdit.AddListener((string val) => {
				int index;
				if (System.Int32.TryParse(val, out index)) {
					gameManager.LoadGameType(index);
				}
			});
		#else
			inputField.gameObject.SetActive(false);
		#endif
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
