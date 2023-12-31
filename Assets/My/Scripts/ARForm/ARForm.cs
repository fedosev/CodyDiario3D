﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// @tmp:
using UnityEngine.UI;

namespace ARFormOptions {


public class ARForm : MonoBehaviour {

	// @tmp
	// public GameConfig tmpGameConfig;

	protected ARMarkerFormContainer formContainer;
	public ARMarkerFormContainer FormContainer { set {
		formContainer = value;
		foreach (var elm in formElements) {
			elm.FormContainer = value;
		}
	} }

	public UnityEvent AfterSubmit;
	public UnityEvent onAfterCheck;

	private ARFormElement[] formElements;

	void Awake() {

		formElements = GetComponentsInChildren<ARFormElement>();

		if (onAfterCheck == null) {
			onAfterCheck = new UnityEvent();
		}
	}

	public void CheckElements() {

		foreach (var elm in formElements) {
			elm.CheckValues();
		}
		if (onAfterCheck != null) {
			onAfterCheck.Invoke();
		}
	}

	public void Submit() {

		foreach (var elm in formElements) {
			elm.SubmitElement();
		}

		// @tmp {
			/*
			var borderSave = GameObject.Find("BorderSave");
			var quadSave = GameObject.Find("QuadSave");
			if (borderSave && quadSave) {
				borderSave.GetComponent<Image>().color = tmpGameConfig.GetBorderColor();
				quadSave.GetComponent<Image>().color = tmpGameConfig.GetQuadColor();
			}
			*/
		// }

		if (AfterSubmit != null) {
			AfterSubmit.Invoke();
		}
	}


}

public abstract class ARFormElement : MonoBehaviour {

	protected ARMarkerFormContainer formContainer;
	public ARMarkerFormContainer FormContainer { set {
		formContainer = value;
	} }

	protected bool IsBetter(float c1, float c2) {
		return c1 < c2;
	}


	public abstract void CheckValues();

	public abstract void SubmitElement();

}

public class ARFormElementValue<t> {

	public const int numSamples = 15;

	private t[] vals = new t[numSamples];
	private int index = 0;

	private bool hasAllSamples = false;

	private Dictionary<t, int> valsCounts = new Dictionary<t, int>();

	// /*
	private t defaultValue;

	public ARFormElementValue(t defaultValue) {
		this.defaultValue = defaultValue;
	}
	// */

	public void SetValue(t val) {
		vals[index] = val;
		if (!hasAllSamples && index + 1 == numSamples)
			hasAllSamples = true;
		index = (index + 1) % numSamples;
	}

	public bool TryGetValue(out t key) {

		key = defaultValue;
		if (!hasAllSamples)
			return false;
		
		valsCounts.Clear();
		for (int i = 0; i < numSamples; i++) {
			int val;
			if (valsCounts.TryGetValue(vals[i], out val)) {
				valsCounts[vals[i]] = val + 1;
			} else {
				valsCounts.Add(vals[i], val + 1);
			}
		}

		int maxCount = 0;
		t maxFormElmVal = vals[0];
		foreach (KeyValuePair<t, int> kvp in valsCounts) {
			if (kvp.Value > maxCount)
				maxFormElmVal = kvp.Key;
		}
		key = maxFormElmVal;

		return true;
	}
	
}


public class ARFormElementRadio : ARFormElement {

	public override void CheckValues() {

	}

	public override void SubmitElement() {

		
	}	

}


}

