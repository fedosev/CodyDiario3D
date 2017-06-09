using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ARFormOptions
{

public class OutputPixelsFormElement : ARFormElement {

	public RawImage outImage;

	float width = 0; 

	Texture2D tex;

	public override void CheckValues() {

        float ratioObject = (float)formContainer.gridCols / (float)formContainer.gridRows;

		if (width == 0)
			width = outImage.rectTransform.rect.width;
        if (ratioObject > Screen.width / Screen.height) {
			outImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * ratioObject);
        } else {
			outImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, width / ratioObject);
        }


        // Debug texture
        if (outImage != null) {
            if (!tex || formContainer.screenWidth != Screen.width || formContainer.screenHeight != Screen.height) {
                tex = new Texture2D(formContainer.gridCols, formContainer.gridRows);
                tex.filterMode = FilterMode.Point;
            }

            for (var i = 0; i < formContainer.gridCols; i++) {
                for (var j = 0; j < formContainer.gridRows; j++) {
                    float avgGr = formContainer.GetAvgGrayscale(new Vec2(i, j));
                    tex.SetPixel(i, j, new Color(avgGr, avgGr, avgGr, 1f));
                }
            }
            tex.Apply();
            outImage.GetComponent<RawImage>().texture = tex;
        }		
	}

}

}