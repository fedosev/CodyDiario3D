using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DayButton : MonoBehaviour, IPointerClickHandler {

	BaseGameType gameType;
	
	public void Init(BaseGameType gameType) {
		this.gameType = gameType;
		GetComponentInChildren<Text>().text = gameType.name;
	}

	public void OnPointerClick(PointerEventData eventData) {
		MainGameManager.Instance.LoadGameType(gameType);
		//DaySelector.Instance.Hide();
		//MainGameManager.Menu.ToggleMenu();
	}

}
