using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyAR;
using ARFormOptions;

public class ScanOptionsManager : BaseGameTypeManager {

	private static ScanOptionsManager instance;
	public static ScanOptionsManager Instance { get {
		if (instance == null)
			instance = GameObject.FindObjectOfType<ScanOptionsManager>();
		return instance;
	} }

	public ImageTargetBehaviour imageTargetOptions;
	public GameObject optionsTargetCanvas;

	public GameConfig gameConfig;

	public UnityEngine.UI.Image borderPreview;
	public UnityEngine.UI.Image quadPreview;


	public const int allBools = 2047;

	public GridColorsFormElement colorsFormElm;
	public GridTransparenciesFormElement transparenciesFormElm;

	public GameObject checkBetterInfo;

	int areGood = 0;

	bool isGood = false;
    bool wasGood = false;

	float allGoodChangedTime = -10f;


	Color borderColor = new Color(0f, 0f, 0f, 0f);
	Color quadColor = new Color(0f, 0f, 0f, 0f);

	IEnumerator showInfoDelayedCoroutine;

    public override GameObject TargetCanvas { get {
		return optionsTargetCanvas;
	} }


	public override void InitConfig() {

		SetOptionsActive(true);

		//colorsFormElm = FindObjectOfType<GridColorsFormElement>();
		colorsFormElm.onBorderPreviewChange.AddListener(UpdateBorderColor);
		colorsFormElm.onQuadPreviewChange.AddListener(UpdateQuadColor);
		colorsFormElm.onSubmit.AddListener(SubmitColors);

		//transparenciesFormElm = FindObjectOfType<GridTransparenciesFormElement>();
		transparenciesFormElm.onBorderPreviewChange.AddListener(UpdateBorderTransparency);
		transparenciesFormElm.onQuadPreviewChange.AddListener(UpdateQuadTransparency);
		transparenciesFormElm.onSubmit.AddListener(SubmitTransparencies);

		//showInfoDelayedCoroutine = ShowInfoDelayed();
	}

	public void UpdateOptions() {
		gameManager.mainMenu.InitOptions();
		gameManager.mainMenu.ShowOptions();
	}

	public void SetOptionsActive(bool activate) {

		gameManager.SetMainImgTargetsActive(!activate, true);

		if (activate) {
			if (!imageTargetOptions.ActiveTargetOnStart) {
				imageTargetOptions.SetupWithImage(imageTargetOptions.Path, imageTargetOptions.Storage, imageTargetOptions.Name, imageTargetOptions.Size);
				imageTargetOptions.TargetFound += (TargetAbstractBehaviour behaviour) => {
					optionsTargetCanvas.SetActive(false);
				};
				imageTargetOptions.TargetLost += (TargetAbstractBehaviour behaviour) => {
					optionsTargetCanvas.SetActive(true);
				};
				imageTargetOptions.ActiveTargetOnStart = true;
			}
			imageTargetOptions.Bind(gameManager.imageTracker);
			imageTargetOptions.gameObject.SetActive(false);
		}
		else { // Deactivate
			if (imageTargetOptions) {
				gameManager.imageTracker.UnloadImageTargetBehaviour(imageTargetOptions);
				optionsTargetCanvas.SetActive(false);
			}
		}
		UpdateVisibility(true);
	}

	public void UpdateBorderColor(Color color, bool isGood) {
		if (isGood) {
			areGood |= 1;
			if ((areGood & 3) == 3) {
				borderColor = new Color(color.r, color.g, color.b, borderColor.a);
			}
		} else {
			areGood &= allBools - 1;
		}
	}

	public void UpdateBorderTransparency(float alpha, bool isGood) {
		if (isGood) {
			areGood |= 2;
			if ((areGood & 3) == 3) {
				borderColor = new Color(borderColor.r, borderColor.g, borderColor.b, alpha);
			}
		} else {
			areGood &= allBools - 2;
		}
	}

	public void UpdateQuadColor(Color color, bool isGood) {
		if (isGood) {
			areGood |= 4;
			if ((areGood & 12) == 12) {
				quadColor = new Color(color.r, color.g, color.b, quadColor.a);
			}
		} else {
			areGood &= allBools - 4;
			
		}
	}

	public void UpdateQuadTransparency(float alpha, bool isGood) {
		if (isGood) {
			areGood |= 8;
			if ((areGood & 12) == 12) {
				quadColor = new Color(quadColor.r, quadColor.g, quadColor.b, alpha);
			}
		} else {
			areGood &= allBools - 8;
		}
	}

	public IEnumerator ShowInfoDelayed() {
		yield return new WaitForSeconds(0.25f);
		//borderPreview.color = new Color(0f, 0.25f, 0.5f, 0.5f);
		//quadPreview.color = Color.clear;
		checkBetterInfo.SetActive(true);
	}

	public void UpdateInfo() {

		var isTimeOk = Time.time - allGoodChangedTime > 1.5f;

		if (isTimeOk) {
			if (isGood && (areGood & 15) != 15) {
				/*
				if (showInfoDelayedCoroutine == null) {
					showInfoDelayedCoroutine = ShowInfoDelayed();
					//MyDebug.Log("StartCoroutine " + areGood, true);
					StartCoroutine(showInfoDelayedCoroutine);
					//allGoodChangedTime = Time.time;
				}
				*/
				checkBetterInfo.SetActive(true);
				isGood = false;
				allGoodChangedTime = Time.time;
			} else if (!isGood && (areGood & 15) == 15) { // All good
				/*
				if (showInfoDelayedCoroutine != null) {
					StopCoroutine(showInfoDelayedCoroutine);
					showInfoDelayedCoroutine = null;
				}
				*/
				checkBetterInfo.SetActive(false);
				allGoodChangedTime = Time.time;
				isGood = true;
				wasGood = true;
				//MyDebug.Log("StopCoroutine " + areGood, true);
			}
		}
		if (wasGood) {
			quadPreview.color = quadColor;
			borderPreview.color = borderColor;
		}
	}

	public void SubmitColors(Color borderCol, Color quadCol) {
		gameConfig.borderColor = borderCol;
		gameConfig.quadColor = quadCol;
	}

	public void SubmitTransparencies(float borderAlpha, float quadAlpha) {
		gameConfig.borderColorAlpha = borderAlpha;
		gameConfig.quadColorAlpha = quadAlpha;
        gameConfig.Save();
	}


	void OnDestroy() {
		if (gameManager != null)
			gameManager.SetMainImgTargetsActive(true);
		instance = null;
	}
	
}
