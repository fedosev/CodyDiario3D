using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class PathGameType : BaseGridRobyGameType {

		public string[] path;
		
		public string code; //@todo maybe not needed

		public override void InitBody() {

			grid.gameType = GameTypes.PATH;
			grid.playersNumber = 1;

			grid.Init();


			if (code.Length > 0) {
				var codingGrid = GameObject.FindObjectOfType<CodingGrid>();
				if (codingGrid != null) {
					for (var i = 0; i < code.Length; i++) {
						codingGrid.AppendCard(code[i]);
					}
				}
			}

			if (path.Length == 0 && grid.state.IsNull()) {
				grid.state = grid.state.GoToState<StateSwitchQuad>();
			}
	    }

		public override void SetupQuad(QuadBehaviour quad, int col, int row) {

			base.SetupQuad(quad, col, row);

			if (path.Length > 0) {
				if (path[grid.nRows - 1 - row][col] != '0') {
					quad.SetState(QuadStates.PATH);
				}
			}
		}

}