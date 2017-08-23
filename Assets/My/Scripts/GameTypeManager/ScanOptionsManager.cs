using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyAR;

public class ScanOptionsManager : BaseGameTypeManager {

	private static ScanOptionsManager instance;
	public static ScanOptionsManager Instance { get {
		if (instance == null)
			instance = GameObject.FindObjectOfType<ScanOptionsManager>();
		return instance;
	} }

	public ImageTargetBehaviour imageTargetOptions;
	public GameObject optionsTargetCanvas;

	public override void InitConfig() {

		SetOptionsActive(true);
	}

	public void UpdateOptions() {
		gameManager.mainMenu.InitOptions();
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

	void OnDestroy() {
		if (gameManager != null)
			gameManager.SetMainImgTargetsActive(true);
		instance = null;
	}
	
}
