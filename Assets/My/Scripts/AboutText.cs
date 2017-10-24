using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class AboutText : MonoBehaviour, IPointerClickHandler {

	TextMeshProUGUI text;


	public void OnPointerClick(PointerEventData eventData) {
		var linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, eventData.pressEventCamera);
		if (linkIndex != -1) {
			var linkInfo = text.textInfo.linkInfo[linkIndex];
			switch (linkInfo.GetLinkID()) {
				case "codemooc":
					Application.OpenURL("http://codemooc.org/diario/");
					break;
				case "fedosev1":
				case "fedosev2":
					var hash = "";
					#if UNITY_EDITOR
						hash = "#editor";
					#elif UNITY_ANDROID
						hash = "#android";
					#elif UNITY_IOS
						hash = "#ios";
					#endif
					Application.OpenURL("https://fedosev.com/codydiario3d/" + hash);
					break;
			}			
		}
	}

	void Awake() {
		text = GetComponent<TextMeshProUGUI>();
	}

}
