using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class SensorsGameType : BaseGridRobyGameType {

	public override string title { get {
		return "Sensori";
	} }

	public override string generalInfo { get {
		return "Oggi Roby è programmato con il codice descritto nel diario. " +
			"Devi trovare la sequenza che eseguirà Roby e la sua posizione finale.\n" + 
			"1. Premi sulla casella della scacchiera per selezionare la posizione finale.\n" +
			"2. Premi sulle carte per aggiungerle alla sequenza di istruzioni e poi premi su \"Esegui\".\n";
	} }

	[HideInInspector] public CodingGrid codingGrid;

	public string[] obstacles;

	const int instructionsSize = 20;
	CardTypes[] instructions;
	int instructionIndex;
	CardTypes[] actions = new CardTypes[instructionsSize];

	QuadBehaviour selectedQuad;

	public override void InitBody() {

		instructionIndex = 0;
		codingGrid = gridRobyManager.codingGrid;
		instructions = codingGrid.GetInstructions();

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

		codingGrid.Show();
		codingGrid.executeButton.onClick.RemoveAllListeners();
		codingGrid.executeButton.onClick.AddListener(Execute);
		codingGrid.SetMaxCardsNumber(instructionsSize);
		codingGrid.HideExec();
		codingGrid.OnChange += (num) => {
			if (num == instructionsSize && selectedQuad != null) {
				gridRobyManager.codingGrid.HideExec(false);
			} else {
				gridRobyManager.codingGrid.HideExec();
			}
		};

		grid.state.GoToState<StateGridSelectQuad>();
		((StateGridSelectQuad)grid.state).OnSelect += OnSelectQuad;
	}

	void OnSelectQuad(GameObjectState state, QuadBehaviour quad) {
		if (quad.mainState != QuadStates.DEFAULT)
			return;
		if (selectedQuad != null) {
			selectedQuad.Undo();
		}
		quad.RecordUndo();
		quad.SetOtherState(QuadStates.INFO);
		selectedQuad = quad;
		grid.SetActiveUIAnimated(true);
	}

	public override void SetupQuad(QuadBehaviour quad, int col, int row) {

		base.SetupQuad(quad, col, row);

		if (obstacles.Length > 0) {
			var obstaclesRow = obstacles[grid.nRows - 1 - row];
			if (obstaclesRow.Length > 0 && obstaclesRow[col] != '0') {
				quad.SetState(QuadStates.OBSTACLE);
			}
		}
	}

	void Execute() {
		grid.state.GoToState<StateNull>();
		codingGrid.HideUI();
		grid.SetActiveUIAnimated(false);
		NextAction();
	}

	void NextAction() {

		if (instructionIndex == instructionsSize) {
			var equal = true;
			for (int i = 0; i < instructionsSize; i++) {
				if (actions[i] != instructions[i]) {
					equal = false;
					break;
				}
			}
			if (equal && grid.GetQuadPositionInGrid(selectedQuad.gameObject).Equals(
				grid.GetQuadPositionInGrid(grid.CurrentRobotController.CurrentQuad)
			)) {
				gridRobyManager.WinAction();
			} else {
				gridRobyManager.LoseAction();
			}
			return;
		}

		CardTypes action;

		var robot = grid.CurrentRobotController;
		if (robot.CanMoveForward()) {
			action = CardTypes.FORWARD;
		} else {
			if (robot.CanGoLeft()) {
				action = CardTypes.LEFT;
			} else {
				action = CardTypes.RIGHT;
			}
		}
		actions[instructionIndex++] = action;
		grid.AddAction(action);
		grid.NextAction();
	}

    public override void ChangeQuad(RobotController robot, QuadBehaviour prevQuad, QuadBehaviour nextQuad) {
        
		prevQuad.SetOtherState(QuadStates.DEFAULT);
		nextQuad.SetOtherState(QuadStates.ACTIVE);
    }

    public override bool QuadIsFreeToGoIn(QuadBehaviour quad) {
        return quad.mainState != QuadStates.OBSTACLE;
    }

}