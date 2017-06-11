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
		
		Load();
	}

	public void Save() {

		Debug.Log("GameConfig - Save");
		PlayerPrefs.SetString("GameConfig", JsonUtility.ToJson(this));

	}

	public void Load() {

		var jsonStr = PlayerPrefs.GetString("GameConfig");
		if (jsonStr.Length > 0) {
			JsonUtility.FromJsonOverwrite(jsonStr, this);
		}

		Debug.Log("GameConfig - Load");
		
	}

}
