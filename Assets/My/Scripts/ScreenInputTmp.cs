using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyARSample;

public class ScreenInputTmp : MonoBehaviour {


	private float time = 0;
	private TargetOnTheFly ui;

	public float doubleClickTime = 0.3f;

	// Use this for initialization
	void Start () {
		ui = FindObjectOfType<TargetOnTheFly>();
	}
	
	// Update is called once per frame
	void Update () {
		if (ui.isShowUI == false && Input.GetMouseButtonDown(0)) {
			if (Time.time - time < doubleClickTime) {
				ui.isShowUI = !ui.isShowUI;

			} else {
				time = Time.time;
			}
		}
	}
}
