using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class AllGameTypes : ScriptableObject {

    public List<BaseGameType> items = new List<BaseGameType>();

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