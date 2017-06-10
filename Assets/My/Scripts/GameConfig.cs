using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameConfig : ScriptableObject {

	public Color quadColor;
	public Color borderColor;
	[Range(0f, 1f)]
	public float quadColorAlpha = 0.5f;
	[Range(0f, 1f)]
	public float borderColorAlpha = 0.5f;

	private Color ColorWithAlpha(Color color, float alpha) {
		color.a = alpha;
		return color;
	}
	public Color GetQuadColor() {
		return ColorWithAlpha(quadColor, quadColorAlpha);
	}

	public Color GetBorderColor() {
		return ColorWithAlpha(borderColor, borderColorAlpha);
	}


	public void Init() {

		var config = ScriptableObject.FindObjectOfType<ConfigScriptableObject>();
		/*
		config.borderMaterial.color = borderColor;
		config.quadMaterial.color = quadColor;
		*/
	}

	public void Save() {

		Debug.Log("GameConfig - Save");

		quadColor.a = quadColorAlpha;
		borderColor.a = borderColorAlpha;
		
	}

}
