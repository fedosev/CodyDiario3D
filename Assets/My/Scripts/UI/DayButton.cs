using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DayButton : MonoBehaviour, IPointerClickHandler {

	BaseGameType gameType;
	MyDate date;
	
	public void Init(BaseGameType gameType, MyDate date) {
		this.gameType = gameType;
		GetComponentInChildren<Text>().text = gameType.name;
		this.date = date;
		if (!MainGameManager.Instance.today.IsGTE(date)) {
			GetComponent<Image>().color = DaySelector.Instance.disabledColor;
		}
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (MainGameManager.Instance.today.IsGTE(date)) {
			MainGameManager.Instance.LoadGameType(gameType);
			SoundManager.Instance.PlayMenu();
			//DaySelector.Instance.Hide();
			//MainGameManager.Menu.ToggleMenu();
		}
	}

}
