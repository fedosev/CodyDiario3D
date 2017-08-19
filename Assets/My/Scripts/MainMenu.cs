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

	// @tmp
	public InputField InputFieldiOS;
	public bool isVisible = false;
	
	public GameObject coverButton;
	public GameObject resumeButton;
	public GameObject helpButton;

	InfoPanel infoPanel;

	public InfoPanel InfoPanel { get {
		if (infoPanel == null)
			infoPanel = panels[infoPanelIndex].GetComponent<InfoPanel>();

		return infoPanel;
	} }
	
	MainGameManager gameManager;

	int panelIndex = -1;
	int prevPanelIndex = -1;
    bool disableFadeIn = false;


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
		
		resumeButton.SetActive(false);
		helpButton.SetActive(false);

		menuButton.onClick.AddListener(ShowMain);
		InfoPanel.onClose += Hide;
	}

	public void ShowMain() {
		prevPanelIndex = -1;
		panelIndex = mainPanelIndex;
		StartCoroutine(ShowAnimated(true, true, mainPanelIndex));
	}

	public void ShowPanel(int panelIndex) {
		prevPanelIndex = panelIndex == mainPanelIndex ? -1 : mainPanelIndex;
		this.panelIndex = panelIndex;
		StartCoroutine(ShowAnimated(true, true, panelIndex));
	}

	public void ShowInfo() {
		ShowPanel(infoPanelIndex);
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

		gameManager.PauseGame(show);
		if (animated && !disableFadeIn)
			yield return StartCoroutine(gameManager.FadeOverlay(true, 0.2f));
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
			yield return StartCoroutine(gameManager.FadeOverlay(false, 0.5f));

		disableFadeIn = false;
		yield return null;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (isVisible && panelIndex == mainPanelIndex) {
				gameManager.Quit();
			} else if (!isVisible) {
				ShowMain();
			} else {
				Back();
			}
		}
	}
}
