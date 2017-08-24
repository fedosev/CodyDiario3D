using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ARFormOptions
{

public class UnityEventColorBool : UnityEvent<Color, bool> { }

public class UnityEvent2Colors : UnityEvent<Color, Color> { }


public class GridColorsFormElement : ARFormElement {

	public GridColors gridColors;

	public GameConfig gameConfig;

    public UnityEventColorBool onBorderPreviewChange;
    public UnityEventColorBool onQuadPreviewChange;
    public UnityEvent2Colors onSubmit;

    float minChangeToApply = 0;

	GridColor bestBorderGC = null;
	GridColor bestGC = null;

    ARFormElementValue<Color> borderColorValue = new ARFormElementValue<Color>(new Color());
    ARFormElementValue<Color> quadColorValue = new ARFormElementValue<Color>(new Color());


	void Awake() {

		if (onBorderPreviewChange == null)
			onBorderPreviewChange = new UnityEventColorBool();
		if (onQuadPreviewChange == null)
			onQuadPreviewChange = new UnityEventColorBool();
		if (onSubmit == null)
			onSubmit = new UnityEvent2Colors();

	}

	public override void CheckValues() {

        if (minChangeToApply == 0f)
             minChangeToApply = formContainer.minChangeToApply * 2;

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

        if (bestBorderGC != null) {
            var isGood = formContainer.minChangeToApply <= (prevBestBorderColor - bestBorderColor);
            if (isGood)
                borderColorValue.SetValue(bestBorderGC.color);

            onBorderPreviewChange.Invoke(bestBorderGC.color, isGood);
        }

        if (bestGC != null) {
            var isGood = formContainer.minChangeToApply <= (prevBestColor - bestColor);
            if (isGood)
                quadColorValue.SetValue(bestGC.color);
            
            onQuadPreviewChange.Invoke(bestGC.color, isGood);
            //Debug.Log(bestGC.name);
        }

        //uiText.text = "Border: " + (int)((prevBestBorderColor - bestBorderColor) * 100) + ", Quad: (" + (int)((prevBestColor - bestColor) * 100) + ")";

	}

	public override void SubmitElement() {

        Color quadColor, borderColor;
        if (quadColorValue.TryGetValue(out quadColor) && borderColorValue.TryGetValue(out borderColor)) {

            onSubmit.Invoke(borderColor, quadColor);
        }
	}	

}


}