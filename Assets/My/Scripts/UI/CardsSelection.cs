using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsSelection : MonoBehaviour, ICardsContainer {

	public GameObject cardLeftPrefab;
	public GameObject cardForwardPrefab;
	public GameObject cardRightPrefab;
	public Button okButton;

	public event Action<CardTypes[]> OnUseCards;

	public float interpolationSpeed = 15f;
	public float padding = 20f;
	float col0;
	float[] cols;
	float row0;
	float row1;

	CardInHand[] cardsInHand;
	CardInHand[] cardsSelected;

	int maxCards;
	int nCardsInHand;
	int nSelectedCards;

	Deck deck;

	public void Init(Deck deck, int n = 5, float offsetY = 0f) {

		if (cardsInHand != null) {
			foreach (var card in cardsInHand) {
				if (card != null)
					Destroy(card.gameObject);
			}
		}
		this.deck = deck;
		this.maxCards = n;
		nCardsInHand = 0;
		cardsInHand = new CardInHand[n];
		cardsSelected = new CardInHand[n];
		nSelectedCards = 0;

		cols = new float[n];
		Vector3[] corners = new Vector3[4];
		var rt = GetComponent<RectTransform>();
		rt.GetWorldCorners(corners);
		/*
		var ratio = corners[3].x - corners[0].x;
		rt.GetLocalCorners(corners);
		ratio = (corners[3].x - corners[0].x) / ratio;
		*/
		var cardRect = cardLeftPrefab.GetComponent<RectTransform>().rect;
		var canvasScale = GetComponentInParent<Canvas>().transform.localScale.x;
		col0 = -40f - cardRect.width * canvasScale;
		row0 = corners[0].y + offsetY * canvasScale;
		row1 = corners[0].y + (cardRect.height + padding + offsetY) * canvasScale;
		for (int i = 0; i < n; i++) {
			cols[i] = corners[0].x + (cardRect.width + padding) * canvasScale * i;
		}
		okButton.onClick.AddListener(UseCards);
		okButton.transform.position = new Vector3(okButton.transform.position.x, row0, 0f);
		okButton.gameObject.SetActive(false);
	}

	public void TakeCards() {

		CardTypes? card;
		for (int i = 0; i < maxCards; i++) {
			card = deck.TakeCard();
			if (card == null)
				return;
			nCardsInHand++;
			InstantiateCard(i, card, true);
		}
	}

	void UseCards() {

		var cards = new CardTypes[nSelectedCards];

		for (var i = nSelectedCards; i < nCardsInHand; i++) {
			deck.ReturnCard();
			
		}
		for (int i = 0; i < nSelectedCards; i++) {
			cards[i] = cardsSelected[i].type;
		}
		deck.UseAllCards();
		okButton.gameObject.SetActive(false);

		if (OnUseCards != null) {
			OnUseCards(cards);
		}

		foreach (var card in cardsInHand) {
			if (card != null)
				Destroy(card.gameObject);
		}
		nCardsInHand = 0;
		nSelectedCards = 0;
		Array.Clear(cardsInHand, 0, maxCards);
		Array.Clear(cardsSelected, 0, maxCards);
	}

	public void InstantiateCard(int i, CardTypes? type, bool animation = false) {

		GameObject cardObj;
		switch (type) {
			case CardTypes.LEFT: cardObj = Instantiate(cardLeftPrefab); break;
			case CardTypes.FORWARD: cardObj = Instantiate(cardForwardPrefab); break;
			case CardTypes.RIGHT: cardObj = Instantiate(cardRightPrefab); break;
			default: cardObj = null; break;
		}
		if (cardObj != null) {
			cardObj.transform.SetParent(transform, false);
			//cardObj.GetComponent<RectTransform>().anchoredPosition = positions[i];
			cardObj.transform.position = new Vector3(cols[i], 0f ,0f);
			var card = cardObj.GetComponent<CardInHand>();
			card = cardObj.GetComponent<CardInHand>();
			if (animation)
				card.SetPosition(new Vector3(col0, row0 ,0f), true);
			card.SetPosition(new Vector3(cols[i], row0 ,0f), true);
			card.Init(this, i);
			cardsInHand[i] = card;
		} else {
			cardsInHand[i] = null;
		}
	}	

	public void HandleClick(CardInHand card) {

		if (card.isSelected) {
			card.SetPosition(new Vector3(cols[card.index], row0, 0f));
			for (int i = Array.FindIndex(cardsSelected, c => c == card) + 1; i < nSelectedCards; i++) {
				cardsSelected[i].SetPosition(new Vector3(cols[i - 1], row1, 0f));
				cardsSelected[i - 1] = cardsSelected[i];
			}
			nSelectedCards--;
		} else {
			card.SetPosition(new Vector3(cols[nSelectedCards], row1, 0f));
			cardsSelected[nSelectedCards] = card;
			nSelectedCards++;
		}
		card.isSelected = !card.isSelected;

		var maxIndex = 0;
		for (var i = 0; i < nSelectedCards; i++) {
			if (maxIndex < cardsSelected[i].index)
				maxIndex = cardsSelected[i].index;
		}

		okButton.gameObject.SetActive(nSelectedCards == maxIndex + 1);
	}

	public float GetInterpolationSpeed() {
		return interpolationSpeed;
	}

	void Start() {

		//Init(new Deck(40));
		okButton.gameObject.SetActive(false);
	}

}
