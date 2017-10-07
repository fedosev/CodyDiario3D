using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Deck {

	public int cardsInHand = 0;

	DeckCard[] cards;
	int nCards;
	int index = 0;

	public Deck(int n) {

		nCards = n;
		cards = new DeckCard[n];
	}

	public Deck(string cardsStr) {

		nCards = cardsStr.Length;
		cards = new DeckCard[nCards];
		for (var i = 0; i < nCards; i++) {
			cards[i] = new DeckCard(CodingGrid.GetTypeFromLetter(cardsStr[i]));
		}
	}

    // public int CardsInHand { get => cardsInHand; }

    public CardTypes? TakeCard() {

		if (index < nCards) {
			var card = cards[index++];
			card.isInHand = true;
			cardsInHand++;
			return card.type;
		}
		return null;
	}

	public void UseCard(int indexInHand) {

		if (cardsInHand > 0 && indexInHand < cardsInHand) {
			var i = 0;
			foreach (var card in cards) {
				if (card.isInHand) {
					if (indexInHand == i) {
						card.isInHand = false;
						return;
					} else {
						i++;
					}
				}

			}
			cardsInHand--;
		}
	}

	public string GetRichText() {
		
		StringBuilder sb = new StringBuilder();

		for (var i = 0; i < nCards; i++) {
			if (i < index) {
				sb.Append(cards[i].isInHand ? "<u>" : "<s>");
				sb.Append(CodingGrid.WrapWithColor(CodingGrid.GetLetterFromType(cards[i].type)));
				sb.Append(cards[i].isInHand ? "</u>" : "</s>");
			} else {
				sb.Append(CodingGrid.WrapWithColor(CodingGrid.GetLetterFromType(cards[i].type)));
			}
		}

		return sb.ToString();
	}


}


public class DeckCard {

	public readonly CardTypes type;
	public bool isInHand = false;

	public DeckCard(CardTypes type) {
		this.type = type;
	}

}
