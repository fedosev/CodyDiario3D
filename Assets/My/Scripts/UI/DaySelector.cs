using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DaySelector : MonoBehaviour {

	static DaySelector instance;

	public static DaySelector Instance { get {
		if (instance == null)
			instance = GameObject.FindObjectOfType<DaySelector>();
		return instance;
	} }
	
	public Text monthText;
	public GameObject daysPanel;

	public GameObject dayButtonPrefab;

	string monthName;

	AllGameTypes.Month month;

	public void Show(string monthName) {

		if (this.monthName != monthName) {
			this.monthName = monthName;
			monthText.text = monthName;
			if (MainGameManager.Instance.allGameTypes.TryGetMonth(monthName, out month)) {
				InitDays();
			}
		}
		gameObject.SetActive(true);
	}

	public void Hide() {

		gameObject.SetActive(false);
	}

	public void InitDays() {

		GameObject dayObj;
		BaseGameType day;

		for (var i = 0; i < daysPanel.transform.childCount; i++) {
			Destroy(daysPanel.transform.GetChild(i).gameObject);
		}
		//foreach (BaseGameType day in month.days) {
		for (var i = 0; i < month.days.Count; i++) {
			day = month.days[i];
			dayObj = Instantiate(dayButtonPrefab);
			dayObj.transform.SetParent(daysPanel.transform, false);
			dayObj.GetComponent<DayButton>().Init(day);
		}
	}

	void Start () {

		Show("Settembre");
	}
}
