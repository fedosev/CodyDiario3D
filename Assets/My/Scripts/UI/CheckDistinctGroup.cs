using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CheckDistinctGroup : MonoBehaviour {

	public CheckDistinct[] items;

	public void SetValue(int index) {
		for (int i = 0; i < items.Length; i++) {
			items[i].SetOn(false);
		}
		items[index].SetOn(true);
	}

	public void Init() {
		if (items.Length == 0) {
			items = GetComponentsInChildren<CheckDistinct>();
		}
		for (int i = 0; i < items.Length; i++) {
			items[i].Init(this, i);
		}
	}

	// Use this for initialization
	void Start () {
		Init();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
