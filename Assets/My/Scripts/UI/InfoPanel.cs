using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {

	public Text title;
	public Text subTitle;
	public Text content;
	public Button closeButton;

	public event Action onClose;
	public event Action OnFirstClose;

	bool wasClosed = false;

	public void Setup(string titleStr, string subTitleStr, string contentStr) {
		title.text = titleStr;
		subTitle.text = subTitleStr;
		content.text = contentStr;
	}

	void Start () {

		closeButton.onClick.AddListener(() => {
			if (onClose != null)
				onClose();
		});
		onClose += OnFirstClose;
	}

	void OnDisable() {
		if (!wasClosed || OnFirstClose != null) {
			OnFirstClose();
			wasClosed = true;
		}
	}
	
}
