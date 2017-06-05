using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public GameObject gridGameObj;
	public InputField gridColsInput;
	public InputField gridRowsInput;
	public Toggle persistentMode;
	public Toggle moveGrid;
	public Grid grid;
	public GameObject panelConfig;
	public GameObject panelControlls;

    protected bool bordersVisible;

    public void OpenConfigMenu() {
		panelConfig.SetActive(true);
		panelControlls.SetActive(false);
		grid.switchQuadStateBehaviour.enabled = true;
		grid.inPause = true;
	}

	public void CloseConfigMenu() {
		panelConfig.SetActive(false);
		panelControlls.SetActive(true);
		grid.movableBehaviour.isActive = false;
		moveGrid.isOn = false;
		grid.switchQuadStateBehaviour.enabled = false;
		grid.inPause = false;
	}

	public void StartGameSnake() {
		grid.gameType = GameTypes.SNAKE;
		UpdateGrid();
	}
	public void StartGamePath() {
		grid.gameType = GameTypes.PATH;
		UpdateGrid();
	}

	public void UpdateGrid() {
		int cols, rows;
		if (int.TryParse(gridColsInput.text, out cols) && int.TryParse(gridRowsInput.text, out rows)) {
			grid.config.gridNumberX = cols;
			grid.config.gridNumberZ = rows;
			grid.ClearGrid();
			grid.GenerateGrid();
            grid.UpdateSize();
		}
	}

	public void GridAddCol() {
		if (grid.config.gridNumberX >= 20)
			return;
		gridColsInput.text = "" + (grid.config.gridNumberX + 1);
		UpdateGrid();
	}
	public void GridRemoveCol() {
		if (grid.config.gridNumberX <= 1)
			return;
		gridColsInput.text = "" + (grid.config.gridNumberX - 1);
		UpdateGrid();
	}
	public void GridAddRow() {
		if (grid.config.gridNumberZ >= 20)
			return;
		gridRowsInput.text = "" + (grid.config.gridNumberZ + 1);
		UpdateGrid();
	}
	public void GridRemoveRow() {
		if (grid.config.gridNumberZ <= 1)
			return;
		gridRowsInput.text = "" + (grid.config.gridNumberZ - 1);
		UpdateGrid();
	}

	public void SwitchMoveGrid() {
		grid.movableBehaviour.isActive = moveGrid.isOn;
		grid.switchQuadStateBehaviour.enabled = !moveGrid.isOn;
	}

    public void SwitchCamera() {

        Debug.Log("Switching Camera...");
    }

    public void FlipCamera() {

        Debug.Log("Flipping Camera...");
    }

    public void SwitchFocusMode() {
        
        Debug.Log("SwitchFocusMode()");
    }

    public void TriggerFocus() {
        Debug.Log("TriggerFocus()");
    }

    public void SwitchBorderVisibility() {
        bordersVisible = !bordersVisible;
        grid.ShowBorders(bordersVisible);
    }

	public void LaunchConfig() {
		using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			jo.Call("launchPreferencesActivity");
		}
	}


    // Use this for initialization
    void Start () {
		//gridColsInput.text = "" + grid.config.gridNumberX;
		//gridRowsInput.text = "" + grid.config.gridNumberZ;

        bordersVisible = true;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
