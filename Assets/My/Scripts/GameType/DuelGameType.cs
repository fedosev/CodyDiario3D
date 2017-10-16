using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class DuelGameType : BaseGridRobyGameType {

	public override string title { get {
		return "Duello";
	} }

	public override string generalInfo { get {
		return "@todo";
	} }

    public string deck = "";

    const int cardsNumber = 5;

    public override void InitBody() {

        grid.gameType = GameTypes.DUEL;
        grid.playersNumber = 2;

		grid.OnNextTurn += NextTurn;
		grid.OnLose += Lose;

        grid.Init();

		grid.SetActiveUI(false);
		grid.SetRobotsColliderActive(true);

        
        if (deck.Length > 0) {
            gridRobyManager.deck = new Deck(deck);
		} else {
			//gridRobyManager.deck = new Deck("ASADASDASAADASDASAADASDASADSADSAADASDAAD");
			gridRobyManager.deck = new Deck("ADASAADSADAASDAADAAASADAAAADSSAASAADSAAA");
		}
		gridRobyManager.codingGrid.gameObject.SetActive(true);
		gridRobyManager.codingGrid.HideUI();
		gridRobyManager.codingGrid.text.SetText(gridRobyManager.deck.GetRichText());

		gridRobyManager.cardsSelection.Init(gridRobyManager.deck, cardsNumber, withLetters ? 0f : -100f);
		gridRobyManager.cardsSelection.OnUseCards += UseCards;
		gridRobyManager.cardsSelection.TakeCards();
		gridRobyManager.codingGrid.text.SetText(gridRobyManager.deck.GetRichText());
    }

    void UseCards(CardTypes[] cards) {

        for (var i = 0; i < cards.Length; i++) {
            grid.AddAction(cards[i]);
        }
        grid.NextAction();
		gridRobyManager.codingGrid.text.SetText(gridRobyManager.deck.GetRichText());
    }

    void NextTurn() {

        if (gridRobyManager.deck.RemainingCards() <= 0) {
            gridRobyManager.WinTextAction("Finite le carte...");
        } else {
			gridRobyManager.cardsSelection.TakeCards();
			gridRobyManager.codingGrid.text.SetText(gridRobyManager.deck.GetRichText());
		}
    }

    public override void ChangeQuad(RobotController robot, QuadBehaviour prevQuad, QuadBehaviour nextQuad) {
        
		prevQuad.SetOtherState(QuadStates.DEFAULT);

		if (nextQuad.otherState == QuadStates.ON) {
            nextQuad.SetState(QuadStates.ACTIVE);
			grid.ClearActions();
			robot.OnStopMove += () => {
				gridRobyManager.WinTextAction("IL GIOCATORE " + (robot.index + 1) + " VINCE!");
			};
		} else if (nextQuad.IsFreeToGoIn()) {
            nextQuad.SetState(QuadStates.ACTIVE);
        } else {
            nextQuad.SetState(QuadStates.ERROR);
            robot.DoLose();
        }
    }

    public override bool QuadIsFreeToGoIn(QuadBehaviour quad) {
        return true;
    }

}