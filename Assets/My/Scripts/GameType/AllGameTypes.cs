using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class AllGameTypes : ScriptableObject {

    [System.Serializable]
    public class Month {
        public string name;
        public bool active = false;
        public int month;
        public int year = 2018;
        public List<BaseGameType> days = new List<BaseGameType>();

    }

    public List<Month> months = new List<Month>();
    public List<BaseGameType> items = new List<BaseGameType>();

    public MyDate startDate;
    public MyDate endDate;


    public bool TryGetValue(string key, out BaseGameType value) {
        foreach (var item in items) {
            if (item.name == key) {
                value = item;
                return true;
            }
        }
        value = null;
        return false;
    }

    public bool TryGetMonth(int monthNumber, out Month value) {
        foreach (var item in months) {
            if (item.month == monthNumber) {
                value = item;
                return true;
            }
        }
        value = null;
        return false;
    }

    public void Save() {
        MyDebug.Log("AllGameTypes - Save");
        PlayerPrefs.SetString("AllGameTypes", JsonUtility.ToJson(this));
        //Debug.Log(JsonUtility.ToJson(this));
    }

    public void Load() {
		var jsonStr = PlayerPrefs.GetString("AllGameTypes");
		if (jsonStr.Length > 0) {
			JsonUtility.FromJsonOverwrite(jsonStr, this);
		}
		MyDebug.Log("AllGameTypes - Load");
    }

	public void OnDisable() {

        //Save();
	}

	public void OnEnable() {

        //Load();
		
	}    

}