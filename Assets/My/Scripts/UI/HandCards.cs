using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ICardsContainer {

	float GetInterpolationSpeed();
	void HandleClick(CardInHand card);
}


public class HandCards : MonoBehaviour, ICardsContainer {

	public GameObject cardLeftPrefab;
	public GameObject cardForwardPrefab;
	public GameObject cardRightPrefab;

	public Func<CardInHand, bool> CanUseCard;
	public event Action<CardInHand> OnUseCard;
	public event Action OnEmptyDeck;

	public CardInHand[] cards;

	public float interpolationSpeed = 15f;

	Vector3[] positions;
	Vector3 distance;
	Deck deck;

	public void Init(Deck deck, int n) {

		this.deck = deck;
		var childCount = transform.childCount;
		var cardTypes = new CardTypes?[n];
		cards = new CardInHand[n];
		for (var i = 0; i < n; i++) {
			cardTypes[i] = deck.TakeCard();
		}
		positions = new Vector3[n];
		for (var i = 0; i < childCount; i++) {
			//var t = transform.GetChild(i).GetComponent<RectTransform>();
			var t = transform.GetChild(i);
			if (i < n) {
				//positions[i] = t.anchoredPosition;
				positions[i] = t.position;
			}
			Destroy(t.gameObject);
		}
		for (int i = 0; i < n; i++) {
			InstantiateCard(i, cardTypes[i]);
		}
		distance = positions[1] - positions[0];
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
			cardObj.transform.position = positions[i];
			var card = cardObj.GetComponent<CardInHand>();
			card = cardObj.GetComponent<CardInHand>();
			if (animation)
				card.SetPosition(positions[i] + distance, true);
			card.SetPosition(positions[i]);
			card.Init(this, i);
			cards[i] = card;
		} else {
			cards[i] = null;
		}
	}

	public void UseCard(CardInHand card) {

		if (CanUseCard == null || !CanUseCard(card))
			return;

		deck.UseCard(card.index);
		var newCardType = deck.TakeCard();

        var index = card.index;
        card.Remove();
        for (var i = index + 1; i < cards.Length && cards[i] != null; i++) {
			cards[i].SetPosition(positions[i - 1]);
			if (cards[i].index > 0)
            	cards[i].index--;
            cards[i - 1] = cards[i];
        }

		if (newCardType != null) {
			InstantiateCard(cards.Length - 1, newCardType, true);
		} else {
			cards[cards.Length - 1] = null;
		}

		if (OnUseCard != null) {
			OnUseCard(card);
		}
		if (cards[0] == null && OnEmptyDeck != null) {
			OnEmptyDeck();
		}
			
	}

	public void HandleClick(CardInHand card) {
		UseCard(card);
	}

	public float GetInterpolationSpeed() {
		return interpolationSpeed;
	}

	public bool AllForward() {
		foreach (var card in cards) {
			if (card != null) {
				if (card.type != CardTypes.FORWARD)
					return false;
			}
		}
		return true;
	}
	
}

