using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DaySelector : MonoBehaviour {

	static DaySelector instance;
	public static DaySelector Instance { get {
		if (instance == null)
			instance = MainGameManager.Menu.GetComponentInChildren<DaySelector>(true);
		return instance;
	} }
	
	public Text monthText;
	public GameObject daysPanel;

	public GameObject dayButtonPrefab;

	public Color disabledColor = new Color(0.53f, 0.53f, 0.53f);
	public Color enabledColor = new Color(0.53f, 0.53f, 0.53f);

	int monthNumber;

	AllGameTypes.Month month;

	bool wasInit = false;
	bool daysWereInit = false;

    List<AllGameTypes.Month> months;
    MonthButton[] monthButtons;

	DayButton[] dayButtons;

	public void RefreshNextTime() {
		wasInit = false;
	}

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
		} else {
			InitDays();
		}
		gameObject.SetActive(true);
	}

	public void Hide() {

		gameObject.SetActive(false);
	}

	public float _delay = 0.028f;
	public float _speed = 7f;
	public float _offset = 6;

	public void InitDays() {

		GameObject dayObj;
		BaseGameType day;

		if (dayButtons == null) {
			var dayButtonsCount = daysPanel.transform.childCount;
			dayButtons = new DayButton[dayButtonsCount];
			for (var i = 0; i < dayButtonsCount; i++) {
				//Destroy(daysPanel.transform.GetChild(i).gameObject);
				dayButtons[i] = daysPanel.transform.GetChild(i).GetComponent<DayButton>();
			}
		}
		for (var i = 0; i < month.days.Count; i++) {
			day = month.days[i];
			var dayB = dayButtons[i];
			dayB.gameObject.SetActive(true);
			dayB.transform.SetParent(daysPanel.transform, false);
			dayB.Init(day, new MyDate(day.year, day.month, i * 2 + 1));
			if (daysWereInit) {
				if (i < 8)
					dayB.Animate(i * _delay, _speed);
				else
					dayB.Animate((i - _offset) * _delay, _speed);
				//dayB.Animate((i % 8) * 0.016f);
				//dayB.Animate(0f);
			}
		}
		daysWereInit = true;
		for (var i = month.days.Count; i < dayButtons.Length; i++) {
			dayButtons[i].gameObject.SetActive(false);
		}
	}

	public void Init() {

		if (!wasInit) {
			months = MainGameManager.Instance.allGameTypes.months;
			monthButtons = GetComponentsInChildren<MonthButton>();
		}
		var i = 0;
		var monthsCount = months.Count;

		foreach (var mb in monthButtons) {
			if (i < monthsCount) {
				mb.isActive = months[i].active;
				i++;
			} else {
				mb.isActive = false;
			}
			mb.Init();
		}
		Show(MainGameManager.Instance.today.month);
		wasInit = true;
	}

	void OnEnable() {

		if (!wasInit)
			Init();
	}

	void Start () {

		//Show(MainGameManager.Instance.today.month);
	}
}
