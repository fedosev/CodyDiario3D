using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class GridHexaGameType : BaseGridRobyGameType {

	public override string title { get {
		return "Esadecimale";
	} }
    
	public override string generalInfo { get {
        var str = "";;
        if (isFreeMode)
            str += "Codifica esadecimale in modalità libera. \nOggi puoi disegnare sulla griglia con il tap continuo.\n";
        else
            str += "Divertiti con la codifica esadecimale.\n";
        if (isGridToCode || isFreeMode)
            str += "Puoi fare il tap sulle caselle della griglia per annerirle (secondo tap per anullare l'azione). Vedrai il risultato della codifica in tempo reale.\n";
        if (!isGridToCode || isFreeMode)
            str += "Puoi scrivere il codice usando la tastiera. Vedrai il risultato sulla griglia in tempo reale.\n";
        return str;
	} }

    public bool isGridToCode = true;
    public bool isFreeMode = false;

    protected GridHexaManager gridHexaManager;

    void OnQuadStateChange(QuadBehaviour quad) {

        int i = quad.index;
        int row = 7 - i / 8;
        int col = i % 8;
        int codeIndex = row * 2 + col / 4;
        int codeBinPos = 3 - col % 4;

        int num = 1;
        while (codeBinPos > 0) {
            num *= 2;
            codeBinPos --;
        }

       gridHexaManager.UpdateHexaCode(codeIndex, num, quad.mainState != QuadStates.DEFAULT);
    }

    public void UpdateQuads(int fieldIndex, int val) {

        int row = 7 - fieldIndex / 2;
        int startIndex = row * 8 + (fieldIndex % 2) * 4 + 3;

        int num = 1;
        for (var i = 0; i < 4; i++) {
            var quad = grid.GetQuadBh(startIndex - i);
            if ((val & num) == num) {
                quad.SetState(QuadStates.OBSTACLE);
            } else {
                quad.SetState(QuadStates.DEFAULT);
            }
            num *= 2;
        }
    }

    public override void BeforeInit() {
        base.BeforeInit();
        gridRobyManager = GridHexaManager.Instance;
        gridHexaManager = GridHexaManager.Instance;
    }

    public override void InitBody() {

        grid.gameType = GameTypes.TAP;
        grid.playersNumber = 0;

        grid.Init();

        if (isGridToCode || isFreeMode) {
            grid.state = grid.state.InitState<StateHexaSwitchQuad>();
            ((StateHexaSwitchQuad)grid.state).isDrawMode = isFreeMode;
            grid.OnQuadStateChange += OnQuadStateChange;
        }
    }

    public override string sceneName { get {
        return "GridHexa";
    } }    

}