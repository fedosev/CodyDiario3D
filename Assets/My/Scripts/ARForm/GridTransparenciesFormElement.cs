using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARFormOptions
{

public class GridTransparenciesFormElement : ARFormElement {

	public GridTransparencies gridTransparencies;

    public Image uiBorder;
    public Image uiQuad;

	public GameConfig gameConfig;

	public const float defaultValue = 0.5f;

	GridTransparency bestBorderTransparency = null;
	GridTransparency bestQuadTransparency = null;

    ARFormElementValue<float> borderAlphaValue = new ARFormElementValue<float>(defaultValue);
    ARFormElementValue<float> quadAlphaValue = new ARFormElementValue<float>(defaultValue);

	public override void CheckValues() {

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

        if (bestBorderTransparency != null && formContainer.minChangeToApply <= (prevBestBorderColorIntensity - bestBorderColorIntensity)) {
            borderAlphaValue.SetValue(bestBorderTransparency.alphaValue);
			var color = uiBorder.color;
			color.a = bestBorderTransparency.alphaValue;
            uiBorder.color = color;
        }

        if (bestQuadTransparency != null && formContainer.minChangeToApply <= (prevBestColorIntensity - bestColorIntensity)) {
            quadAlphaValue.SetValue(bestQuadTransparency.alphaValue);
			var color = uiQuad.color;
			color.a = bestQuadTransparency.alphaValue;
            uiQuad.color = color;
        }


	}

	public override void SubmitElement() {

        float quadAlpha, borderAlpha;
        if (quadAlphaValue.TryGetValue(out quadAlpha) && borderAlphaValue.TryGetValue(out borderAlpha)) {
			gameConfig.quadColorAlpha = quadAlpha;
			gameConfig.borderColorAlpha = borderAlpha;
        }
		
	}	

}


}