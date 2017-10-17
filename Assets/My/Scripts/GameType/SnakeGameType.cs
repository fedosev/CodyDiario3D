using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class SnakeGameType : BaseGridRobyGameType {

	public override string title { get {
		return "Serpente";
	} }

	public override string generalInfo { get {
		string str;
		if (deck.Length > 0) {
            str = "Oggi il mazzo di carte è nella seguente sequenza:\n";
			str += deck + ".\n";
		} else {
			str = "Oggi puoi usare tutte le istruzioni che vuoi.\n";
		}
		return str;
	} }

    public string deck = "";

    const int cardsNumber = 3;

    public override void InitBody() {

        useFirstQuad = true;

        grid.gameType = GameTypes.SNAKE;
        grid.playersNumber = 1;

		grid.OnNextTurn += CheckWin;
		grid.OnLose += Lose;

        grid.Init();

        grid.CurrentRobotController.score = 1;
        

        if (deck.Length > 0) {
            gridRobyManager.deck = new Deck(deck);
    		gridRobyManager.codingGrid.Show();
            gridRobyManager.codingGrid.HideUI();
            gridRobyManager.codingGrid.text.SetText(gridRobyManager.deck.GetRichText());
            if (gridRobyManager.handCards != null) {
                gridRobyManager.handCards.Init(gridRobyManager.deck, cardsNumber);
                gridRobyManager.handCards.CanUseCard = CanUseCard;
                gridRobyManager.handCards.OnUseCard += UseCard;
                gridRobyManager.handCards.OnEmptyDeck += Win;
                gridRobyManager.codingGrid.text.SetText(gridRobyManager.deck.GetRichText());
            }
        }
    }

    bool CanUseCard(CardInHand card) {
        if (grid.CurrentRobotController.IsMoving())
            return false;
        return true;
    }

    void UseCard(CardInHand card) {
        Card.RobotActionFromCard(grid.CurrentRobotController, card.type);
        gridRobyManager.codingGrid.text.SetText(gridRobyManager.deck.GetRichText());
    }

    void CheckWin() {
        if (grid.CurrentRobotController.score == 25) {
            Win();
        } else if (deck.Length > 0 && gridRobyManager.handCards.AllForward()
            && !grid.CurrentRobotController.CanMoveForward()
        ) {
            Win();
        }
    }

    void Win() {
        //var score = grid.PlayerQuadCount(0);
        var score = grid.CurrentRobotController.score;
        var text = score == 25 ? "WOW! { 25 }" : "{ " + score.ToString() + " }";
        gridRobyManager.WinTextAction(text);
    }

    public override void ChangeQuad(RobotController robot, QuadBehaviour prevQuad, QuadBehaviour nextQuad) {
        
        if (robot.isFirstMove && !useFirstQuad) {
            prevQuad.SetOtherState(QuadStates.DEFAULT);
        } else {
            prevQuad.SetState(QuadStates.OBSTACLE);
            //prevQuad.player = 0;
        }

        if (nextQuad.IsFreeToGoIn()) {
            nextQuad.SetState(QuadStates.ACTIVE);
            //nextQuad.player = 0;
            grid.CurrentRobotController.score++;
        } else {
            nextQuad.SetState(QuadStates.ERROR);
            robot.DoLose();
        }
    }

    public override bool QuadIsFreeToGoIn(QuadBehaviour quad) {
        return quad.mainState != QuadStates.OBSTACLE;
    }

}