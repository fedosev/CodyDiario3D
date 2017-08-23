using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckDistinct : MonoBehaviour {

	int index;
	CheckDistinctGroup group;
	Toggle toggle;
	bool disableOnChange = false;
	ISavable savable;

	void Awake() {
		toggle = GetComponent<Toggle>();
	}

	public void Init(CheckDistinctGroup group, int index) {
		this.group = group;
		this.index = index;
		toggle.onValueChanged.AddListener(OnChange);
		savable = GetComponent<ISavable>();
		disableOnChange = false;
	}

	public void SetOn(bool isOn) {
		disableOnChange = true;
		toggle.isOn = isOn;
		disableOnChange = false;
		//StartCoroutine(EnableOnChange());
	}

	public IEnumerator EnableOnChange() {
		yield return new WaitForSeconds(0.1f);
		disableOnChange = false;
	}

	void OnChange(bool val) {

		if (disableOnChange)
			return;

		if (val == true) {
			group.SetValue(index);
		} else {
			SetOn(true);
		}
		savable.Save();
	}
}
