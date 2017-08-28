using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class GridTapGameType : BaseGridRobyGameType {

	public override string title { get {
		return "La griglia";
	} }
    
	public override string generalInfo { get {
		return "Puoi provare a fare il tap sulle caselle per annerirle";
	} }

    public override void InitBody() {

        grid.gameType = GameTypes.TAP;
        grid.playersNumber = 0;

        grid.Init();

        grid.state = grid.state.InitState<StateSwitchQuad>();
    }

}