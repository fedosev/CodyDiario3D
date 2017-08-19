using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class CoverGameType : FreeModeGameType {

	public override string title { get {
		return "Cover";
	} }

    public override void InitBody() {

		grid.gameType = GameTypes.FREE;
		grid.playersNumber = 1;

		grid.Init();
    }

    public override string sceneName { get {
        return "Cover";
    } }

}