using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class HexaTextField : MonoBehaviour, IPointerClickHandler {

	public TMP_Text text;

	int index;
	HexaText hexaText;

	Renderer rend;
	
	
	public void OnPointerClick(PointerEventData eventData) {
		hexaText.FieldClicked(index);
	}

	public void Init(HexaText hexaText, int index) {
		this.hexaText = hexaText;
		this.index = index;
		rend = GetComponent<Renderer>();
		rend.material.color = hexaText.defaultQuadColor;
		text = GetComponentInChildren<TMP_Text>();
	}

    public void Highlight(bool highlight) {
		if (highlight) {
			rend.material.color = hexaText.highlightQuadColor;
		} else {
			rend.material.color = hexaText.defaultQuadColor;
		}
    }
}
