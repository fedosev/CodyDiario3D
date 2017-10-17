using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

	public void Init() {
		index = 0;
		foreach (var card in cards) {
			card.isInHand = false;
		}
	}

    public void Shuffle() {
		Init();
		var rnd = new System.Random();
		// /*
		var i = cards.Length;
        while (i > 1) {
            int k = rnd.Next(i--);
            var tmp = cards[i];
            cards[i] = cards[k];
            cards[k] = tmp;
			
		}		
		// */
		// cards = cards.OrderBy(x => rnd.Next()).ToArray();
	}

    public CardTypes? TakeCard() {

		if (index < nCards) {
			var card = cards[index++];
			card.isInHand = true;
			cardsInHand++;
			return card.type;
		}
		return null;
	}

    public void ReturnCard() {

		if (cardsInHand > 0) {
			var card = cards[--index];
			card.isInHand = false;
			cardsInHand--;
		}
	}

	public void UseCard(int indexInHand) {

		if (cardsInHand > 0 && indexInHand < cardsInHand) {
			var i = 0;
			foreach (var card in cards) {
				if (card.isInHand) {
					if (indexInHand == i) {
						card.isInHand = false;
						cardsInHand--;
						return;
					} else {
						i++;
					}
				}
			}
		}
	}

	public void UseAllCards() {

		foreach (var card in cards) {
			if (card.isInHand) {
					card.isInHand = false;
					cardsInHand--;
					if (cardsInHand == 0)
						return;
			}
		}
	}

	public int RemainingCards() {
		return nCards - index;
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
