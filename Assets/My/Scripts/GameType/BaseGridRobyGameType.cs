using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RobyDirection { Null, North, East, South, West }

[System.Serializable]
public class RobyStartPosition {

    public int col = -1;
    public int row = -1;
    public RobyDirection direction = RobyDirection.Null;

    public bool IsSet() {
        return col >= 0 && row >= 0 && direction != RobyDirection.Null;
    }
    
    public bool IsSetPosition() {
        return col >= 0 && row >= 0;
    }

    public bool IsSetDirection() {
        return  direction != RobyDirection.Null;
    }

    public Vector3 GetDirection() {
        return GetDirection(this.direction);
    }
    public static Vector3 GetDirection(RobyDirection direction) {
        switch (direction) {
            case RobyDirection.North: return Vector3.forward;
            case RobyDirection.East: return Vector3.right;
            case RobyDirection.South: return Vector3.back;
            case RobyDirection.West: return Vector3.left;
            default: return Vector3.forward;
        }
    }

}


public abstract class BaseGridRobyGameType : BaseGameType {

	public override string generalInfo { get {
        return (withLetters && (letters == null || letters.Length == 0)) ? lettersSelectorInstructions : "";
    } }

    [System.NonSerialized]
    public Grid grid;

    public RobyStartPosition startPosition;
    public RobyStartPosition startPositionP2;

    public bool useFirstQuad = false;
    public bool withLetters = false;

    public string[] letters;


    public bool useDevBoard = false;

    public bool isChristmasPeriod = false;

    protected GridRobyManager gridRobyManager;

    public virtual void SetupQuad(QuadBehaviour quad, int col, int row) { }

    public virtual void ChangeQuad(RobotController robot, QuadBehaviour prevQuad, QuadBehaviour nextQuad) { }

    public virtual void OnInitRobot(RobotController robot, QuadBehaviour quad) {
        if (useFirstQuad && withLetters) {
            quad.UseLetter(0, 0.75f);
        }
    }

    public virtual bool QuadIsFreeToGoIn(QuadBehaviour quad) {
        return false;
    }

    public override void BeforeInit() {
        base.BeforeInit();
        gridRobyManager = GridRobyManager.Instance;
        if (gridRobyManager.codingGrid != null) {
            gridRobyManager.codingGrid.gameObject.SetActive(false);
        }
    }

    public override void Pause(bool pause) {
        if (grid != null) {
            grid.inPause = pause;
            //grid.UIControls.SetActive(!pause);
        }
    }

	public virtual void Lose(int player) {
		gridRobyManager.LoseAction();
	}
    

    public override string sceneName { get {
        return "GridRoby";
    } }

	protected const string devBoardInstructions = 
		"Annerisci bene le caselle opportune del programmatore sulla pagina del diario. " +
		"Assicurati di trovarti in un posto ben illuminato. Stendi bene la pagina con la mano senza coprire il programmatore. " +
		"Con l'altra mano inquadra il programmatore. Quando compariranno i valori premi su \"Usa la sequenza\". " +
		"Se i valori sono stati presi male potrai correggerli nella fase successiva.\n";

	protected const string lettersSelectorInstructions = 
		"Puoi personalizzare gliglia con le lettere a tuo piacimento usando la tastiera.\n";

}