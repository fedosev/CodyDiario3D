using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class AllGameTypes : ScriptableObject {

    [System.Serializable]
    public class Month {
        public string name;
        public bool active = false;
        public List<BaseGameType> days = new List<BaseGameType>();

    }

    public List<Month> months = new List<Month>();
    public List<BaseGameType> testItems = new List<BaseGameType>();

    public bool TryGetValue(string key, out BaseGameType value) {
        foreach (var item in testItems) {
            if (item.name == key) {
                value = item;
                return true;
            }
        }
        value = null;
        return false;
    }

    public bool TryGetMonth(string monthName, out Month value) {
        foreach (var item in months) {
            if (item.name == monthName) {
                value = item;
                return true;
            }
        }
        value = null;
        return false;
    }

    public void Save() {
        Debug.Log("AllGameTypes - Save");
        PlayerPrefs.SetString("AllGameTypes", JsonUtility.ToJson(this));
        //Debug.Log(JsonUtility.ToJson(this));
    }

    public void Load() {
		var jsonStr = PlayerPrefs.GetString("AllGameTypes");
		if (jsonStr.Length > 0) {
			JsonUtility.FromJsonOverwrite(jsonStr, this);
		}
		Debug.Log("AllGameTypes - Load");
    }

	public void OnDisable() {

        //Save();
	}

	public void OnEnable() {

        //Load();
		
	}    

}