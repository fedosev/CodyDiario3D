using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BooleanPreview : MonoBehaviour {

	public GameObject offObj;
	public GameObject onObj;

	public void SetValue(bool val) {
		offObj.SetActive(!val);
		onObj.SetActive(val);
	}

	public void Init() {
		offObj.SetActive(false);
		onObj.SetActive(false);
	}
	void Start() {
		Init();
	}
	
}
