/**
* Copyright (c) 2015-2016 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
* EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
* and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
*/

using UnityEngine;
using System.Collections;
using EasyAR;

namespace EasyARSample
{
    public class TargetOnTheFly : MonoBehaviour
    {
        private const string title = "Please enter KEY first!";
        private const string boxtitle = "===PLEASE ENTER YOUR KEY HERE===";
        private const string keyMessage = ""
            + "Steps to create the key for this sample:\n"
            + "  1. login www.easyar.com\n"
            + "  2. create app with\n"
            + "      Name: TargetOnTheFly (Unity)\n"
            + "      Bundle ID: cn.easyar.samples.unity.targetonthefly\n"
            + "  3. find the created item in the list and show key\n"
            + "  4. replace all text in TextArea with your key";

        [HideInInspector]
        public bool StartShowMessage = false;
        private bool isShowing = false;
        private ImageTargetManager imageManager;
        private FilesManager imageCreater;

        public GUISkin skin;
        public bool isShowBox = true;

        public float width;
        public float height = 0.9f;
        public GameObject UICanvas;

        public bool isShowUI = true;

        public GameObject game;

        private void Awake()
        {
            if (FindObjectOfType<EasyARBehaviour>().Key.Contains(boxtitle))
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayDialog(title, keyMessage, "OK");
#endif
                Debug.LogError(title + " " + keyMessage);
            }
            imageManager = FindObjectOfType<ImageTargetManager>();
            imageCreater = FindObjectOfType<FilesManager>();

            UICanvas = GameObject.Find("UICanvas");
        }

        void Start() {
            
            //width = height * (195f / 145f) * ((float)Screen.height / (float)Screen.width);
        }

        void OnGUI()
        {
            if (!isShowUI)
                return;

            width = height * (195f / 145f) * ((float)Screen.height / (float)Screen.width);

            if (StartShowMessage)
            {
                if (!isShowing)
                    StartCoroutine(showMessage());
                StartShowMessage = false;
            }

            //GUI.Box(new Rect(Screen.width / 2 - 250, 30, 500, 60), "The box area will be used as ImageTarget. Take photo!", skin.GetStyle("Box"));
            if (true || isShowBox)
                GUI.Box(new Rect(Screen.width * (1 - width) / 2, Screen.height * (1 - height) / 2, Screen.width * width, Screen.height * height), "", skin.GetStyle("Box"));

            if (isShowing) {
                GUI.Box(new Rect(Screen.width / 2 - 65, Screen.height / 2, 130, 60), "Photo Saved", skin.GetStyle("Box"));
            }


            if (GUI.Button(new Rect(Screen.width - 200, Screen.height / 2 - 80, 150, 80 * 2), "Take Photo", skin.GetStyle("Button"))) {
                isShowBox = false;
                isShowUI = false;
                UICanvas.active = false;
                game.active = false;
                imageCreater.StartTakePhoto();
            }

            if (GUI.Button(new Rect(Screen.width / 2 - 80, Screen.height - 180, 160, 80 * 2), "Clear Targets", skin.GetStyle("Button")))
            {
                isShowBox = true;
                imageCreater.ClearTexture();
                imageManager.ClearAllTarget();
            }
            if (GUI.Button(new Rect(40, Screen.height - 180, 160, 160), " - ", skin.GetStyle("Button"))) {
                game.transform.localScale /= 1.02f;
            }
            if (GUI.Button(new Rect(40 + 200, Screen.height - 180, 160, 160), " + ", skin.GetStyle("Button"))) {
                game.transform.localScale *= 1.02f;
            }
            if (GUI.Button(new Rect(Screen.width - 200, 40, 150, 80 * 2), "Hide", skin.GetStyle("Button"))) {
                isShowUI = false;
            }
        }

        IEnumerator showMessage()
        {
            isShowing = true;
            yield return new WaitForSeconds(2f);
            isShowing = false;
        }
    }
}
