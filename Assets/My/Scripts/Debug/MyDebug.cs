using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MyDebug {

	public static Action<string> ShowOnScreen;

	static string allLogs = "";
	static string lastLog = "";
	static int index = 0;


	[Conditional("F_ALLOW_DEBUG")]
	public static void Log(object obj, bool showOnScreen = false, bool sameLine = false) {

		#if UNITY_EDITOR
			UnityEngine.Debug.Log(obj);
		#endif

		if (showOnScreen && MainGameManager.Instance) {
			lastLog = (++index) + " \\> " + (obj.ToString()) + "\n";
			if (ShowOnScreen != null)
				if (sameLine) {
					ShowOnScreen(lastLog + allLogs);
				} else {
					allLogs = lastLog + allLogs;
					ShowOnScreen(allLogs);
				}
		}

	}

}
