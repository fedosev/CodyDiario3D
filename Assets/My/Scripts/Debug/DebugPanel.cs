using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class DebugPanel : MonoBehaviour, IPointerClickHandler {

	private static DebugPanel instance;
	public static DebugPanel Instance { get {
		if (instance == null)
			instance = GameObject.FindObjectOfType<DebugPanel>();
		return instance;
	} }	
	float t;
	RectTransform rt;
	bool isExpanded = false;

	void Start() {
		rt = GetComponent<RectTransform>();
	}
	public void OnPointerClick(PointerEventData eventData) {

		if (Time.time - t < 0.4f) {
			 rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, isExpanded ? 12f : 108f);
			 isExpanded = !isExpanded;
		}

		t = Time.time;
	}

	void Update () {
		
	}
}
