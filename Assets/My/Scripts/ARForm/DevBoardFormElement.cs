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
        float prevBestColorIntensity;

		// @tmp:
		StringBuilder sb = new StringBuilder();

		int i;
        for (i = 0; i < boadLength; i++) {
			bestCard = CardType.Null;
			bestColorIntensity = 1f;
			prevBestColorIntensity = 1f;
			for (var j = 0; j < cardsNumber; j++) {
				float c = formContainer.GetAvgGrayscale(new Vec2(i * colDistance + colStarting, (int)CardRows[j]));
				if (IsBetter(c, bestColorIntensity)) {
					prevBestColorIntensity = bestColorIntensity;
					bestColorIntensity = c;
					bestCard = (CardType)j;
				}
			}
			if (bestCard != CardType.Null && minColorIntensityDifference <= (prevBestColorIntensity - bestColorIntensity)) {
				bestCards[i] = bestCard;
				cardsValues[i].SetValue(bestCard);
				sb.Append((int)bestCard);
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
				sb.Append((int)card);
				sb.Append(" ");
			} else {
				break;
			}
		}
		uiSavedValues.text = sb.ToString();

	}	

}


}