//#define F_BENCHMARK_HERE

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

	const int maxConsecutiveForward = 4;
	const int maxConsecutiveNotForward = 3;
	const int maxConsecutiveSameNotForward = 2;


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

    public void Shuffle(bool withConstraints = false) {
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
		if (withConstraints) {
			var maxCycles = 10;

			#if F_BENCHMARK_HERE
				var aCycles = new int[maxCycles + 1];
				for (int l = 0; l < 100000; l++) {
					i = cards.Length;
					while (i > 1) {
						int k = rnd.Next(i--);
						var tmp = cards[i];
						cards[i] = cards[k];
						cards[k] = tmp;
						
					}
			#endif

			CardTypes lastCard = cards[0].type;
			var cycles = 0;
			while (cycles++ < maxCycles) {
				var count = 1;
				var lastIsNotForward = cards[0].type != CardTypes.FORWARD;
				var countNotForward = lastIsNotForward ? 1 : 0;
				var emptyCycle = true;
				for (i = 1; i < nCards; i++) {
					if (cards[i].type != lastCard) {
						count = 1;
						lastCard = cards[i].type;
					} else {
						count++;
					}
					if (cards[i].type == CardTypes.FORWARD) {
						countNotForward = 0;
					} else {
						countNotForward++;
					}
					lastIsNotForward = cards[i].type != CardTypes.FORWARD;

					if (countNotForward <= maxConsecutiveNotForward &&
						(lastCard == CardTypes.FORWARD && count > maxConsecutiveForward ||
						lastCard != CardTypes.FORWARD && count > maxConsecutiveSameNotForward)
					) {
						var j = i;
						while ((j = (j + 1) % nCards) != i) {
							if (!lastIsNotForward && cards[j].type != lastCard ||
								lastIsNotForward && cards[j].type == CardTypes.FORWARD
							) {
								var tmp = cards[i];
								cards[i] = cards[j];
								cards[j] = tmp;
								lastCard = cards[i].type;
								emptyCycle = false;
								break;
							}
						}
					}
					// /*
					else if (lastIsNotForward && countNotForward > maxConsecutiveNotForward) {
						var j = i;
						while ((j = (j + 1) % nCards) != i) {
							if (cards[j].type == CardTypes.FORWARD) {
								var tmp = cards[i];
								cards[i] = cards[j];
								cards[j] = tmp;
								//lastIsNotForward = false;
								countNotForward = 1;
								emptyCycle = false;
								break;
							}
						}
					}
					// */
				}
				if (emptyCycle)
					break;
			}
			#if F_BENCHMARK_HERE
					aCycles[cycles - 1]++;
				}
				var sb = new StringBuilder();
				for (i = 0; i < maxCycles; i++) {
					sb.Append(i + ": " + aCycles[i] + ", ");
				}
				Debug.Log(sb.ToString());
			#else
				MyDebug.Log("Shuffle cycles: " + cycles, true);
			#endif
		}
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
