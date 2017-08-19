using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {

	public Text title;
	public Text content;
	public Button closeButton;

	public event Action onClose;

	public void Setup(string titleStr, string contentStr) {
		title.text = titleStr;
		content.text = contentStr;
	}

	void Start () {

		closeButton.onClick.AddListener(() => {
			if (onClose != null)
				onClose();
		});
	}
	
}
