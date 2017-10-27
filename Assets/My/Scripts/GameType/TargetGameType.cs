using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


[CreateAssetMenu]
public class TargetGameType : BaseGridRobyGameType {

	public override string title { get {
		return "Sensori";
	} }

	public override string generalInfo { get {
		return "Oggi Roby è programmato con il codice descritto nel diario. " +
			"Devi aggiungere le stelline, usandone il minor numero possibile, per far arrivare Roby alla casella con il cerchietto.\n" + 
			"Premi sulla caselle della scacchiera per aggiungere le stelline e poi premi su \"Esegui\".\n";
	} }

	public string[] obstacles;

	public PositionInGrid endPosition;

	int starsNumber;

	//List<CardTypes> actions = new List<CardTypes>(); // Just in case...
    private bool shouldTakeStar;

    public override void InitBody() {

		starsNumber = 0;
		shouldTakeStar = false;
		//actions.Clear();

		if (!startPosition.IsSet()) {
			startPosition.col = 1;
			startPosition.row = 2;
			startPosition.direction = RobyDirection.East;
		}
		useFirstQuad = true;

		grid.gameType = GameTypes.AUTO;
		grid.playersNumber = 1;

		grid.OnNextTurn += NextAction;
		grid.OnLose += Lose;

		grid.Init();

		grid.state.GoToState<StateGridSelectQuad>();
		((StateGridSelectQuad)grid.state).OnSelect += OnSelectQuad;

		gridRobyManager.execButton.gameObject.SetActive(true);
		gridRobyManager.execButton.onClick.AddListener(() => {
			Execute();
		});
	}

	void OnSelectQuad(GameObjectState state, QuadBehaviour quad) {

		if (quad.mainState == QuadStates.DEFAULT && quad.otherState == QuadStates.DEFAULT) {
			starsNumber++;
			//quad.SetState(QuadStates.STAR);
			quad.SetStateAnimated(QuadStates.STAR, true);
		} else if (quad.mainState == QuadStates.STAR) {
			starsNumber--;
			quad.SetState(QuadStates.DEFAULT);
			//quad.SetStateAnimated(QuadStates.DEFAULT, false, 10f, 0.04f);
		}
	}

	public override void SetupQuad(QuadBehaviour quad, int col, int row) {

		base.SetupQuad(quad, col, row);

		if (endPosition.col == col && endPosition.row == row) {
			quad.SetState(QuadStates.CIRCLE);
		} else if (obstacles.Length > 0) {
			var obstaclesRow = obstacles[grid.nRows - 1 - row];
			if (obstaclesRow.Length > 0 && obstaclesRow[col] != '0') {
				quad.SetState(QuadStates.OBSTACLE);
			}
		}
	}

	void Execute() {

		grid.state.GoToState<StateNull>();
		//gridRobyManager.execButton.GetComponent<RectTransform>().DOAnchorPosY(-120f, 0.4f).OnComplete(() => {
			gridRobyManager.execButton.gameObject.SetActive(false);
		//});
		
		NextAction();
	}

	void NextAction() {

		var robot = grid.CurrentRobotController;
		var currentQuad = robot.CurrentQuad.GetComponent<QuadBehaviour>();

		if (currentQuad.mainState == QuadStates.CIRCLE) {
			gridRobyManager.WinAction();
			return;
		}
		
		if (shouldTakeStar) {
			//currentQuad.SetMainState(QuadStates.DEFAULT);
			currentQuad.SetStateAnimated(QuadStates.DEFAULT, false, 10f, 0.04f);
			currentQuad.SetState(QuadStates.ACTIVE, true);
			SoundManager.Instance.PlayStar();
			grid.nextActionDelay = 0.75f;
			shouldTakeStar = false;
		}		

		CardTypes action;

		if (currentQuad.mainState == QuadStates.STAR) {
			action = CardTypes.LEFT;
			shouldTakeStar = true;
		} else if (robot.CanMoveForward()) {
			action = CardTypes.FORWARD;
		} else {
			action = CardTypes.RIGHT;
		}
		//actions.Add(action);
		grid.AddAction(action);
		grid.NextAction();
	}

    public override void ChangeQuad(RobotController robot, QuadBehaviour prevQuad, QuadBehaviour nextQuad) {

		prevQuad.SetState(QuadStates.DEFAULT);
		if (nextQuad.mainState != QuadStates.STAR) {
			nextQuad.SetState(QuadStates.ACTIVE);
		}
    }

    public override bool QuadIsFreeToGoIn(QuadBehaviour quad) {
        return quad.mainState != QuadStates.OBSTACLE;
    }

}