using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HexaText : MonoBehaviour {

	public HexaTextField[] items;

	public const char blankChar = '_';

	public event Action<int> OnFieldClick;

	public Color highlightQuadColor;
	public Color defaultQuadColor;

	public void Init(bool withBlank) {
		var str = withBlank ? blankChar.ToString() : "0";
		var i = 0;
		foreach (var item in items) {
			item.Init(this, i++);
			item.text.text = str + str;
		}
	}

    public void FieldClicked(int index) {
		if (OnFieldClick != null)
        	OnFieldClick(index);
    }

    public void SetText(int index, int val) {
		string str;
		if (val <= 9) {
			str = val.ToString();
		} else {
			str = ((char)(87 + val)).ToString();
		}
		var item = items[index / 2];
		if (index % 2 == 0) {
			str += item.text.text[1];
		} else {
			str = item.text.text[0] + str;
		}
		item.text.text = str;
	}

    public void Highlight(int editIndex) {
        foreach (var item in items) {
			item.Highlight(false);
		}
		items[editIndex / 2].Highlight(true);
    }

	void Awake() {
		//Init();
	}

}
