using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class PathBackGameType : BaseGridRobyGameType {

	public override string title { get {
		return "Andata e ritorno";
	} }

	public override string generalInfo { get {
		var str = "Oggi Roby è programmato con la sequenza:\n";
		str += code + ".\n";
		str += "Devi trovare la sequenza di ritorno.\n";
		if (MainGameManager.Instance && MainGameManager.Instance.useAR) {
			str += devBoardInstructions;
		} else {
			str += "Premi sulle carte per aggiungerle alla sequenza di istruzioni e premi su \"Esegui\".\n";
		}
		return str;
	} }

	// [Header("Path")]
	
	public string code;

	bool ignoreCheckPath;

	[HideInInspector] public CodingGrid codingGrid;

	public override void InitBody() {

		if (!startPosition.IsSet()) {
			startPosition.col = 2;
			startPosition.row = 2;
			startPosition.direction = RobyDirection.East;
		}
		useFirstQuad = true;
		ignoreCheckPath = true;

		grid.gameType = GameTypes.PATH;
		grid.playersNumber = 1;

		grid.OnNextTurn += ExecuteBackCode;
		grid.OnLose += Lose;

		grid.Init();

		gridRobyManager.codingGrid.Show();
		gridRobyManager.codingGrid.executeButton.onClick.RemoveAllListeners();
		gridRobyManager.codingGrid.executeButton.onClick.AddListener(ExecuteForwardCode);
	}

	void ExecuteForwardCode() {
		// /*
			if (grid.inPause)
					return;
		// */
		for (var i = 0; i < code.Length; i++) {
				grid.AddAction(CodingGrid.GetTypeFromLetter(code[i]));
		}
		grid.NextAction();
		grid.SetActiveUIAnimated(false);
		gridRobyManager.codingGrid.HideUI();
	}

	void ExecuteBackCode() {
		ignoreCheckPath = false;
		grid.OnNextTurn -= ExecuteBackCode;
		grid.OnNextTurn += CheckWin;
		gridRobyManager.codingGrid.Execute();
	}

	public override void SetupQuad(QuadBehaviour quad, int col, int row) {
		base.SetupQuad(quad, col, row);
	}

	public override void OnInitRobot(RobotController robot, QuadBehaviour quad) {
		base.OnInitRobot(robot, quad);
	}

	public override void ChangeQuad(RobotController robot, QuadBehaviour prevQuad, QuadBehaviour nextQuad) {
			
		prevQuad.SetState(QuadStates.PATH);
		prevQuad.SetOtherState(QuadStates.DEFAULT);
				
		if (ignoreCheckPath) { // Forward
			if (robot.isFirstMove) {
				prevQuad.number = 0;
			}
			nextQuad.number = prevQuad.number + 1;
		} else { // Back
			if (nextQuad.number != prevQuad.number - 1 && nextQuad.number != prevQuad.number + 1) {
				robot.DoLose();
			}
		}

		if (nextQuad.IsFreeToGoIn()) {
				nextQuad.SetState(QuadStates.ACTIVE);
				nextQuad.player = 0;
		} else {
				nextQuad.SetState(QuadStates.ERROR);
				robot.DoLose();
		}
	}

	public override bool QuadIsFreeToGoIn(QuadBehaviour quad) {
			return ignoreCheckPath || quad.mainState == QuadStates.PATH;
	}

	public void CheckWin() {

		if (grid.GetQuadPositionInGrid(grid.CurrentRobotController.CurrentQuad).Equals(new PositionInGrid(startPosition.row, startPosition.col))) {
			gridRobyManager.WinAction();
		} else {
			gridRobyManager.LoseAction();
		}
	}

}