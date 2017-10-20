using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DayButton : MonoBehaviour, IPointerClickHandler {

	BaseGameType gameType;
	MyDate date;

	Image image;
	Text text;

	Vector3 pos;
	Color color;
	Color textColor;
	Coroutine anim;

	bool wasInit = false;
	bool isAnimating = false;
	
	public void Init(BaseGameType gameType, MyDate date) {
		this.gameType = gameType;
		GetComponentInChildren<Text>().text = gameType.name;
		this.date = date;
		if (!MainGameManager.Instance.today.IsGTE(date)) {
			GetComponent<Image>().color = DaySelector.Instance.disabledColor;
		}
		if (!wasInit) {
			image = GetComponent<Image>();
			text = GetComponentInChildren<Text>();
			pos = transform.localPosition;
			color = image.color;
			textColor = text.color;
			wasInit = true;
		}
	}

	public void Animate(float delay, float speed = 15f) {
		if (pos != Vector3.zero) {
			transform.localPosition = pos;
		}
		if (anim != null) {
			StopCoroutine(anim);
		}
		anim = StartCoroutine(AnimateEnum(delay, speed));
	}
	
	public IEnumerator AnimateEnum(float delay, float speed) {
		isAnimating = true;
		//transform.localPosition += new Vector3(660f, 0, 0f);
		transform.localPosition += new Vector3(20f, 0, 0f);
		//transform.localPosition = new Vector3(560f, pos.y, pos.z);
		image.color = new Color(color.r, color.g, color.b, 0f);
		text.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
		yield return new WaitForSeconds(delay);
		//while (transform.localPosition.x > pos.x + 0.5f) {
		while (image.color.a < 0.99f) {
			var sp = Time.deltaTime * speed;
			transform.localPosition = Vector3.Lerp(transform.localPosition, pos, sp);
			image.color = Color.Lerp(image.color, color, sp * 0.5f);
			text.color = Color.Lerp(text.color, textColor, sp * 0.5f);
			yield return null;
		}
		transform.localPosition = pos;
		image.color = color;
		text.color = textColor;
		isAnimating = false;
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (MainGameManager.Instance.today.IsGTE(date)) {
			MainGameManager.Instance.LoadGameType(gameType);
			SoundManager.Instance.PlayMenu();
			//DaySelector.Instance.Hide();
			//MainGameManager.Menu.ToggleMenu();
		}
	}

	void OnDisable() {
		if (isAnimating) {
			StopCoroutine(anim);
			transform.localPosition = pos;
			image.color = color;
			text.color = textColor;
		}
	}

}
