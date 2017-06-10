using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARFormOptions
{

public class GridColorsFormElement : ARFormElement {

	public GridColors gridColors;

    public Image uiBorder;
    public Image uiQuad;
    public Text uiText;

	public GameConfig gameConfig;	

	GridColor bestBorderGC = null;
	GridColor bestGC = null;

    ARFormElementValue<Color> borderColorValue = new ARFormElementValue<Color>(new Color());
    ARFormElementValue<Color> quadColorValue = new ARFormElementValue<Color>(new Color());

	public override void CheckValues() {

        float bestBorderColor = 1f;
        float prevBestBorderColor = 1f;
        float bestColor = 1f;
        float prevBestColor = 1f;

        foreach (GridColor gc in gridColors.items) {
            //Color c = tex2d.GetPixel(gc.posForBorder.x + texOffset.x, gc.posForBorder.y + texOffset.y);
            float c = formContainer.GetAvgGrayscale(gc.posForBorder);
            if (IsBetter(c, bestBorderColor)) {
                prevBestBorderColor = bestBorderColor;
                bestBorderColor = c;
                bestBorderGC = gc;
            }
            c = formContainer.GetAvgGrayscale(gc.posForQuad);
            if (IsBetter(c, bestColor)) {
                prevBestColor = bestColor;
                bestColor = c;
                bestGC = gc;
            }
        }

        if (bestBorderGC != null && formContainer.minChangeToApply <= (prevBestBorderColor - bestBorderColor)) {
            borderColorValue.SetValue(bestBorderGC.color);
            uiBorder.color = bestBorderGC.color;
            //Debug.Log(bestGC.name);
        }

        if (bestGC != null && formContainer.minChangeToApply <= (prevBestColor - bestColor)) {
            quadColorValue.SetValue(bestGC.color);
            uiQuad.color = bestGC.color;
            //Debug.Log(bestGC.name);
        }

        uiText.text = "Border: " + (int)((prevBestBorderColor - bestBorderColor) * 100) + ", Quad: (" + (int)((prevBestColor - bestColor) * 100) + ")";

	}

	public override void SubmitElement() {

        Color quadColor, borderColor;
        if (quadColorValue.TryGetValue(out quadColor) && borderColorValue.TryGetValue(out borderColor)) {
            Debug.Log("Saving colors...");
			gameConfig.borderColor = borderColor;
			gameConfig.quadColor = quadColor;
        }
		
	}	

}


}