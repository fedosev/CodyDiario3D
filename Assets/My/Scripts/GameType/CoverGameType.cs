using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class CoverGameType : FreeModeGameType {

	public override string title { get {
		return "Copertina";
	} }

    public override string subTitle { get {
        return "Questa è una demo. Le sfide inizieranno dal 1 Settembre";
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