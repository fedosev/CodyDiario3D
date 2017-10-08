using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodingGrid : MonoBehaviour {

    public GameObject controlsUI;

    public TextMeshProUGUI text;

    public GameObject removeButton;
    public Button executeButton;
    public GameObject panelCards;

    public event Action<int> OnChange;

    public CardTypes[] GetInstructions() {
        return cards;
    }

    //bool isControlsUIVisible = false;

    int maxCardsNumber;
    CardTypes[] cards;
    int cardsNumber;

    Grid grid;
    RectTransform removeButtonRT;
    readonly Vector2 removeButtonOffset = new Vector2(177f, 0f);
    bool shouldBeHiddenUI = false;
    bool shouldBeHiddenExec = false;

    public void HideUI(bool hide = true) {
        shouldBeHiddenUI = hide;
    }

    public void HideExec(bool hide = true) {
        shouldBeHiddenExec = hide;
    }

    public static char GetLetterFromType(CardTypes cardType) {
        switch (cardType) {
            case CardTypes.FORWARD: return 'A';
            case CardTypes.LEFT: return 'S';
            case CardTypes.RIGHT: return 'D';
        }
        return 'X';
    }

    public static string WrapWithColor(char cardLetter) {
        switch (cardLetter) {
            case 'A': return "<color=#00ff00ff>A</color>";
            case 'S': return "<color=#ffff00ff>S</color>";
            case 'D': return "<color=#ff0000ff>D</color>";
        }
        return cardLetter.ToString();
    }

    public static CardTypes GetTypeFromLetter(char cardLetter) {
        switch (cardLetter) {
            case 'A': return CardTypes.FORWARD;
            case 'S': return CardTypes.LEFT;
            case 'D': return CardTypes.RIGHT;
            default:
                Debug.LogError(cardLetter);
                Debug.LogError("Should be A, S or D");
                return CardTypes.FORWARD;
        }
    }

    public void Init(int maxCards = 25) {
        maxCardsNumber = maxCards;
        executeButton.onClick.AddListener(Execute);
        cards = new CardTypes[maxCardsNumber];
        Clear();
    }

    public void SetMaxCardsNumber(int maxCards) {
        maxCardsNumber = maxCards;
    }

    public void Clear() {
        cardsNumber = 0;
        text.text = "";
        if (OnChange != null)
            OnChange(cardsNumber);
    }

    public void AppendCard(CardTypes type) {

        if (cardsNumber >= maxCardsNumber) 
            return;

        cards[cardsNumber] = type;
        cardsNumber++;

        text.text += (WrapWithColor(GetLetterFromType(type)));
        //MyDebug.Log(text.mesh.vertexCount);
        if (OnChange != null)
            OnChange(cardsNumber);
    }
    public void AppendCard(char cardLetter) {

        if (cardsNumber >= maxCardsNumber)
            return;

        cards[cardsNumber] = GetTypeFromLetter(cardLetter);
        cardsNumber++;

        text.text += (WrapWithColor(cardLetter));
        if (OnChange != null)
            OnChange(cardsNumber);
    }

    public void SetCards(string cardLetters) {
        for (var i = 0; i < cardLetters.Length; i++) {
            AppendCard(cardLetters[i]);
        }
    }    

    public void RemoveLastCard() {
        if (cardsNumber > 0) {
            cardsNumber--;
            text.text = text.text.Substring(0, text.text.Length - 26);
        }
        if (OnChange != null)
            OnChange(cardsNumber);
    }

    public void Execute() {
        if (grid.inPause)
            return;
        for (var i = 0; i < cardsNumber; i++) {
            grid.AddAction(cards[i]);
        }
        cardsNumber = 0;
        grid.state.GoToState<StateNull>();
        grid.NextAction();
    }

    public void DisableEdit(bool disable = true) {
        removeButton.SetActive(!disable);
        panelCards.SetActive(!disable);
    }

    void Start () {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        removeButtonRT = removeButton.GetComponent<RectTransform>();
        //Init();
    }
	
	void Update () {

        // /*
        if (!shouldBeHiddenUI && cardsNumber > 0 && !controlsUI.activeSelf) {
            if (!(grid.state is StateGridPlayerPosition || grid.state is StateGridPlayerDirection))
                controlsUI.SetActive(true);
        } else if ((shouldBeHiddenUI || cardsNumber == 0) && controlsUI.activeSelf) {
            controlsUI.SetActive(false);
        }
        if (shouldBeHiddenExec && executeButton.gameObject.activeSelf) {
            executeButton.gameObject.SetActive(false);
            removeButtonRT.anchoredPosition += removeButtonOffset;
        } else if (!shouldBeHiddenExec && !executeButton.gameObject.activeSelf) {
            executeButton.gameObject.SetActive(true);
            removeButtonRT.anchoredPosition -= removeButtonOffset;
        }

        // */
        /* 
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Execute();
		}
        // */
	}
}
