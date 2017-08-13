using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
//using TMPro;

public class MainMenu : MonoBehaviour {

	public bool isHidden = false;
	public Dropdown gameTypeSelector;
	public GameObject mainMenuPanel;
	public Button menuButton;

	// @tmp
	public InputField InputFieldiOS;
	
	MainGameManager gameManager;
	bool isVisible = false;

	void Awake() {
		gameManager = MainGameManager.Instance;
	}

	public void SetupGameTypeSelector() {

		List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
		Dropdown.OptionData optData;
		foreach (var item in gameManager.allGameTypes.testItems) {
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

		//SetupGameTypeSelector();

		//var InputFieldiOS = GameObject.Find("InputFieldiOS").GetComponent<InputField>();
		/*
		#if UNITY_IOS
			gameTypeSelector.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150f);
			InputFieldiOS.onEndEdit.AddListener((string val) => {
				int index;
				if (System.Int32.TryParse(val, out index)) {
					gameManager.LoadGameType(index);
				}
			});
		#else
			InputFieldiOS.gameObject.SetActive(false);
		#endif
		*/
		
		menuButton.onClick.AddListener(ToggleMenu);
	}

	public void ToggleMenu() {
		Show(!isVisible);
	}

	public void Show(bool show) {
		gameManager.PauseAR(show);
		gameManager.PauseGame(show);
		mainMenuPanel.SetActive(show);
		menuButton.gameObject.SetActive(!show);
		isVisible = show;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (isVisible) {
				gameManager.Quit();
			} else {
				ToggleMenu();
			}
		}
	}
}
