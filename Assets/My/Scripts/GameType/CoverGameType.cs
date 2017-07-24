using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class CoverGameType : FreeModeGameType {

    public override void Init() {

		Debug.Log("Cover InitConfig");

		grid.gameType = GameTypes.FREE;
		grid.playersNumber = 1;

		grid.Init();
    }

    public override string sceneName { get {
        return "Cover";
    } }

}