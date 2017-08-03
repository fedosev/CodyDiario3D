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

    [System.NonSerialized]
    public Grid grid;

    public RobyStartPosition startPosition;

    public bool withLetters = false;

    public string[] letters;

    public virtual void SetupQuad(QuadBehaviour quad, int col, int row) { }

    public virtual void ChangeQuad(RobotController robot, QuadBehaviour prevQuad, QuadBehaviour nextQuad) { }

    public virtual bool QuadIsFreeToGoIn(QuadBehaviour quad) {
        return false;
    }

    public override string sceneName { get {
        return "GridRoby";
    } }

}