using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MonthButton : MonoBehaviour, IPointerClickHandler {

	public string monthName;
	public bool isActive = false;
	
	public void OnPointerClick(PointerEventData eventData) {

		if (isActive) {
			DaySelector.Instance.Show(monthName);
		}
	}
	void Start() {
		
		if (!isActive) {
			GetComponent<Image>().color = new Color(0.53f, 0.53f, 0.53f);
		}
	}

}
