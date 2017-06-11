using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ARFormOptions
{

public class DevBoardFormElement : ARFormElement {

	public const int cardsNumber = 3;
	public enum CardType {
		Left,
		Forward,
		Right,
		Null
	}

	public int[] CardRows = new int[cardsNumber];

	public int colStarting = 6;
	public int colDistance = 2;

	public int rowLeftCard = 7;
	public int rowForwardCard = 5;
	public int rowRightCard = 3;

	public const int boadLength = 13;

	[Range(0f, 1f)]
	public float minColorIntensityDifference = 0.06f;

	public Text uiPreviewValues;
	public Text uiSavedValues;

	float bestColorIntensity;

	CardType bestCard;
	CardType[] bestCards = new CardType[boadLength];
	
	private string GetCardChar(CardType card) {
		switch (card) {
			case CardType.Left: return "S";
			case CardType.Forward: return "A";
			case CardType.Right: return "D";
		}
		return "_";
	}


    ARFormElementValue<CardType>[] cardsValues = new ARFormElementValue<CardType>[boadLength];

	public void Awake() {
		CardRows[(int)CardType.Left] = rowLeftCard;
		CardRows[(int)CardType.Forward] = rowForwardCard;
		CardRows[(int)CardType.Right] = rowRightCard;

		for (int i = 0; i < boadLength; i++) {
			cardsValues[i] = new ARFormElementValue<CardType>(CardType.Null);
		}
	}

	public override void CheckValues() {

        float bestColorIntensity;
        float avgColorIntensity;

		// @tmp:
		StringBuilder sb = new StringBuilder();

		int i;
        for (i = 0; i < boadLength; i++) {
			bestCard = CardType.Null;
			bestColorIntensity = 1f;
			avgColorIntensity = 0f;
			for (var j = 0; j < cardsNumber; j++) {
				float c = formContainer.GetAvgGrayscale(new Vec2(i * colDistance + colStarting, (int)CardRows[j]));
				avgColorIntensity += c;
				if (IsBetter(c, bestColorIntensity)) {
					bestColorIntensity = c;
					bestCard = (CardType)j;
				}
			}
			avgColorIntensity /= cardsNumber;
			if (bestCard != CardType.Null && minColorIntensityDifference <= (avgColorIntensity - bestColorIntensity)) {
				bestCards[i] = bestCard;
				cardsValues[i].SetValue(bestCard);
				sb.Append(GetCardChar(bestCard));
				sb.Append(" ");
			} else {
				cardsValues[i].SetValue(CardType.Null);
				break;
			}
        }
		uiPreviewValues.text = sb.ToString();

	}

	public override void SubmitElement() {

		// @tmp
		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < boadLength; i++) {
			CardType card;
			if (cardsValues[i].TryGetValue(out card) && card != CardType.Null) {
				sb.Append(GetCardChar(card));
				sb.Append(" ");
			} else {
				break;
			}
		}
		uiSavedValues.text = sb.ToString();

	}	

}


}