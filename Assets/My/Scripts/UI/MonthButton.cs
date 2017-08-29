using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MonthButton : MonoBehaviour, IPointerClickHandler {

	public string monthName;
	public bool isActive = false;

	public int year = 2018;
	public int month;
	
	public void OnPointerClick(PointerEventData eventData) {

		if (isActive && MainGameManager.Instance.today.IsMonthGTE(year, month)) {
			DaySelector.Instance.Show(month);
		}
	}
	public void Init() {
		if (!(isActive && MainGameManager.Instance.today.IsMonthGTE(year, month))) {
			GetComponent<Image>().color = DaySelector.Instance.disabledColor;
		}
	}

}
