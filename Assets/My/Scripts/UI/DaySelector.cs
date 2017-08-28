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

	public Color disabledColor = new Color(0.53f, 0.53f, 0.53f);

	int monthNumber;

	AllGameTypes.Month month;


	public void Show(int monthNumber) {

		if (this.monthNumber != monthNumber) {
			var m = monthNumber;
			for (var i = 0; i < 2; i++) {
				do {
					if (MainGameManager.Instance.allGameTypes.TryGetMonth(m, out month)) {
						if (month.active) {
							this.monthNumber = m;
							InitDays();
							monthText.text = month.name;
							i = 2;
							break;
						}
					}
				} while(--m > 0);
				m = 12;
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
		for (var i = 0; i < month.days.Count; i++) {
			day = month.days[i];
			dayObj = Instantiate(dayButtonPrefab);
			dayObj.transform.SetParent(daysPanel.transform, false);
			dayObj.GetComponent<DayButton>().Init(day, new MyDate(day.year, day.month, i * 2 + 1));
		}
	}

	void Start () {

		Show(MainGameManager.Instance.today.month);
	}
}
