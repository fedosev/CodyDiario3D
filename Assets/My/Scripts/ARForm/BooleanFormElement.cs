using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ARFormOptions
{

[System.Serializable]
public class UnityEventBool : UnityEvent<bool> { }

public class BooleanFormElement : ARFormElement {

	public Vec2 posTrue;
	public Vec2 posFalse;

	public UnityEventBool onPreviewChange;
	public UnityEvent onNotAccurate;
	public UnityEventBool onSubmit;

	public bool defaultValueTrue = false;

	//public GameConfig gameConfig;

    ARFormElementValue<bool> boolValue;


	void Awake() {

		boolValue = new ARFormElementValue<bool>(defaultValueTrue);

		if (onPreviewChange == null)
			onPreviewChange = new UnityEventBool();
		if (onNotAccurate == null)
			onNotAccurate = new UnityEvent();
		if (onSubmit == null)
			onSubmit = new UnityEventBool();
	}

	public override void CheckValues() {

		float trueIntensity = formContainer.GetAvgGrayscale(posTrue);
		float falseIntensity = formContainer.GetAvgGrayscale(posFalse);
		bool val = IsBetter(
			formContainer.GetAvgGrayscale(posTrue),
			formContainer.GetAvgGrayscale(posFalse)
		);
        if (formContainer.minChangeToApply <= Mathf.Abs(trueIntensity - falseIntensity)) {
            boolValue.SetValue(val);
			
			onPreviewChange.Invoke(val);
        } else {
			onNotAccurate.Invoke();
		}
	}

	public override void SubmitElement() {

        bool val;
        if (boolValue.TryGetValue(out val)) {
			onSubmit.Invoke(val);
        }
		
	}	

}


}