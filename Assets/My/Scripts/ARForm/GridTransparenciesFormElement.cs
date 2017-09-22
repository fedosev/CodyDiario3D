using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ARFormOptions
{

public class UnityEventFloatBool : UnityEvent<float, bool> { }

public class UnityEvent2Floats : UnityEvent<float, float> { }


public class GridTransparenciesFormElement : ARFormElement {

	public GridTransparencies gridTransparencies;

	//public GameConfig gameConfig;

	public const float defaultValue = 0.5f;

    float minChangeToApply = 0;


	GridTransparency bestBorderTransparency = null;
	GridTransparency bestQuadTransparency = null;

    ARFormElementValue<float> borderAlphaValue = new ARFormElementValue<float>(defaultValue);
    ARFormElementValue<float> quadAlphaValue = new ARFormElementValue<float>(defaultValue);

    public UnityEventFloatBool onBorderPreviewChange;
    public UnityEventFloatBool onQuadPreviewChange;
    public UnityEvent2Floats onSubmit;


	void Awake() {

		if (onBorderPreviewChange == null)
			onBorderPreviewChange = new UnityEventFloatBool();
		if (onQuadPreviewChange == null)
			onQuadPreviewChange = new UnityEventFloatBool();
		if (onSubmit == null)
			onSubmit = new UnityEvent2Floats();
	}

	public override void CheckValues() {

        if (minChangeToApply == 0f)
             minChangeToApply = formContainer.minChangeToApply * 2;

        float bestBorderColorIntensity = 1f;
        float prevBestBorderColorIntensity = 1f;
        float bestColorIntensity = 1f;
        float prevBestColorIntensity = 1f;

        foreach (GridTransparency gt in gridTransparencies.items) {
            float c = formContainer.GetAvgGrayscale(gt.posForBorder);
            if (IsBetter(c, bestBorderColorIntensity)) {
                prevBestBorderColorIntensity = bestBorderColorIntensity;
                bestBorderColorIntensity = c;
                bestBorderTransparency = gt;
            }
            c = formContainer.GetAvgGrayscale(gt.posForQuad);
            if (IsBetter(c, bestColorIntensity)) {
                prevBestColorIntensity = bestColorIntensity;
                bestColorIntensity = c;
                bestQuadTransparency = gt;
            }
        }

        if (bestBorderTransparency != null) {
            var isGood = formContainer.minChangeToApply <= (prevBestBorderColorIntensity - bestBorderColorIntensity);
            if (isGood)
                borderAlphaValue.SetValue(bestBorderTransparency.alphaValue);

            onBorderPreviewChange.Invoke(bestBorderTransparency.alphaValue, isGood);
        }

        if (bestQuadTransparency != null) {
            var isGood = formContainer.minChangeToApply <= (prevBestColorIntensity - bestColorIntensity);
            if (isGood)
                quadAlphaValue.SetValue(bestQuadTransparency.alphaValue);

			onQuadPreviewChange.Invoke(bestQuadTransparency.alphaValue, isGood);
        }


	}

	public override void SubmitElement() {

        float quadAlpha, borderAlpha;
        if (quadAlphaValue.TryGetValue(out quadAlpha) && borderAlphaValue.TryGetValue(out borderAlpha)) {

            onSubmit.Invoke(borderAlpha, quadAlpha);
        }
	}	

}


}