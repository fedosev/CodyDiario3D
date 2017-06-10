using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ARFormOptions {


public class ARForm : MonoBehaviour {

	protected ARMarkerFromContainer formContainer;
	public ARMarkerFromContainer FormContainer { set {
		formContainer = value;
		foreach (var elm in formElements) {
			elm.FormContainer = value;
		}
	} }

	public UnityEvent AfterSubmit;

	private ARFormElement[] formElements;

	void Awake() {

		formElements = GetComponentsInChildren<ARFormElement>();
	}

	public void CheckElements() {

		foreach (var elm in formElements) {
			elm.CheckValues();
		}
	}

	public void Submit() {

		foreach (var elm in formElements) {
			elm.SubmitElement();
		}

		if (AfterSubmit != null) {
			AfterSubmit.Invoke();
		}
	}


}

public abstract class ARFormElement : MonoBehaviour {

	protected ARMarkerFromContainer formContainer;
	public ARMarkerFromContainer FormContainer { set {
		formContainer = value;
	} }

	protected bool IsBetter(float c1, float c2) {
		return c1 < c2;
	}


	public abstract void CheckValues();

	public abstract void SubmitElement();

}

public class ARFormElementRadio : ARFormElement {

	public override void CheckValues() {

	}

	public override void SubmitElement() {

		
	}	

}


}

