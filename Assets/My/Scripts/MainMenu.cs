using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
//using TMPro;

public class MainMenu : MonoBehaviour {

	public Dropdown gameTypeSelector;
	public GameObject[] panels;
	public Button menuButton;

	public int mainPanelIndex = 0;
	public int infoPanelIndex = 1;
	public int daySelectorPanelIndex = 2;
	public int optionsPanelIndex = 3;

	// @tmp
	public InputField InputFieldiOS;
	public bool isVisible = false;
	
	public GameObject coverButton;
	public GameObject daySelectorButton;
	public GameObject resumeButton;
	public GameObject helpButton;

	public Popup popup;


	InfoPanel infoPanel;
	OptionsPanel optionsPanel;

	public InfoPanel InfoPanelObj { get {
		if (infoPanel == null)
			infoPanel = panels[infoPanelIndex].GetComponent<InfoPanel>();
		return infoPanel;
	} }

	public OptionsPanel OptionsPanelObj { get {
		if (optionsPanel == null)
			optionsPanel = panels[optionsPanelIndex].GetComponent<OptionsPanel>();
		return optionsPanel;
	} }
	
	MainGameManager gameManager;

	int panelIndex = -1;
	int prevPanelIndex = -1;
    bool disableFadeIn = false;

    bool isAfterFirstDay = false;


    void Awake() {

		gameManager = MainGameManager.Instance;
		if (isVisible)
			panelIndex = mainPanelIndex;

		popup.Init();
	}

	public void InitOptions() {
		OptionsPanelObj.Init();
	}

	public void AfterFirstGameLoad() {

		var optionsPanelImg = panels[optionsPanelIndex].GetComponent<Image>();
		var optionsPanelColor = optionsPanelImg.color;
		optionsPanelImg.color = new Color(optionsPanelColor.r, optionsPanelColor.g, optionsPanelColor.b, 0.5f);

		resumeButton.SetActive(true);
		helpButton.SetActive(true);
		
	}

	public void CheckAfterFirstDay() {
		var isFirstDay = gameManager.today.IsGTE(gameManager.allGameTypes.startDate);
		coverButton.SetActive(!isFirstDay);
		daySelectorButton.SetActive(isFirstDay);
		isAfterFirstDay = isFirstDay;
	}

	public void SetupGameTypeSelector() {

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
		
		resumeButton.SetActive(false);
		helpButton.SetActive(false);

		menuButton.onClick.AddListener(ShowMain);
		InfoPanelObj.onClose += Hide;

		CheckAfterFirstDay();
	}

	public void ShowMain() {
		prevPanelIndex = -1;
		panelIndex = mainPanelIndex;
		StartCoroutine(ShowAnimated(true, true, mainPanelIndex));
	}

	public void ShowMainOnStart() {
		disableFadeIn = true;
		ShowMain();
	}

	public void ShowPanel(int panelIndex) {
		prevPanelIndex = panelIndex == mainPanelIndex ? -1 : mainPanelIndex;
		this.panelIndex = panelIndex;

		if (!isAfterFirstDay && panelIndex == mainPanelIndex) {
			CheckAfterFirstDay();
		}
		StartCoroutine(ShowAnimated(true, true, panelIndex));
	}

	public void ShowInfo() {
		ShowPanel(infoPanelIndex);
	}

	public void ShowOptions() {
		ShowPanel(optionsPanelIndex);
	}

	public void ShowDaySelector() {
		if (!gameManager.today.IsGTE(gameManager.allGameTypes.startDate)) {
			MainGameManager.Instance.LoadCover();
			return;
		}

		ShowPanel(daySelectorPanelIndex);
	}

	public void ShowInfoOnStart() {
		disableFadeIn = true;
		ShowPanel(infoPanelIndex);
	}

	public void Hide() {
		prevPanelIndex = -1;
		panelIndex = -1;		
		StartCoroutine(ShowAnimated(false, true));
	}

	public void Hide(bool animated) {
		StartCoroutine(ShowAnimated(false, animated));
	}

	public void Back() {
		if (prevPanelIndex != -1)
			ShowPanel(prevPanelIndex);
		else
			Hide();
	}

	public IEnumerator ShowAnimated(bool show, bool animated, int panelIndex = -1) {

		var slowFade = disableFadeIn;
		gameManager.PauseGame(show);
		if (animated && !disableFadeIn) {
			yield return StartCoroutine(gameManager.FadeOverlay(true, 0.2f));
		}
		disableFadeIn = false;
		gameManager.PauseAR(show);
		isVisible = show;
		gameManager.UpdateVisibility();
		foreach (var panel in panels) {
			panel.SetActive(false);
		}
		if (panelIndex != -1)
			panels[panelIndex].SetActive(show);
		menuButton.gameObject.SetActive(!show);
		if (animated)
			yield return StartCoroutine(gameManager.FadeOverlay(false, slowFade ? 2f : 0.5f));

		yield return null;
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (isVisible && panelIndex == mainPanelIndex) {
				//gameManager.Quit();
				if (!gameManager.isBackground)
					Hide();
			} else if (!isVisible) {
				ShowMain();
			} else {
				Back();
			}
		}
	}
}
