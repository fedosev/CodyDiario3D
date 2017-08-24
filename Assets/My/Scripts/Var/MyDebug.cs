using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDebug {

	public static string allLogs = "";
	public static string lastLog = "";
	static int index = 0;


	public static void Log(object obj, bool showOnScreen = false, bool sameLine = false) {

		#if UNITY_EDITOR
			Debug.Log(obj);
		#endif

		if (showOnScreen && MainGameManager.Instance) {
			lastLog = (++index) + " \\> " + (obj.ToString()) + "\n";
			if (sameLine) {
				MainGameManager.Instance.SetLogText(lastLog + allLogs);
			} else {
				allLogs = lastLog + allLogs;
				MainGameManager.Instance.SetLogText(allLogs);
			}
		}

	}

}
