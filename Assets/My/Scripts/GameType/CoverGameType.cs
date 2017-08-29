using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class CoverGameType : FreeModeGameType {

	public override string title { get {
		return "Copertina";
	} }

    string _subTitle;

    public override string subTitle { get {
        return _subTitle;
    } }

    public void SetOtherSubTitle(bool isAfterEnd) {
        if (isAfterEnd)
            _subTitle = "Questa è una demo, non ci sono altri giochi disponibili per oggi";
        else
            _subTitle = "Questa è una demo, le sfide inizieranno dal 1 Settembre";
    }   

    public override void InitBody() {

        _subTitle = "Questa è una demo";

		grid.gameType = GameTypes.FREE;
		grid.playersNumber = 1;

		grid.Init();
    }

    public override string sceneName { get {
        return "Cover";
    } }

}