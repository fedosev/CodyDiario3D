﻿/**
* Copyright (c) 2015-2016 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
* EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
* and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
*/

using UnityEngine;
using EasyAR;

namespace EasyAR
{
    public class MyImageTargetBehaviour : ImageTargetBehaviour
    {
        protected MainGameManager gameManager;

        protected override void Awake()
        {
            base.Awake();
            TargetFound += OnTargetFound;
            TargetLost += OnTargetLost;
            //gameObject = GameObject.Find("Game2").transform.GetChild(0).gameObject;
            /*
            subGameObject = Instantiate(Resources.Load("Game2", typeof(GameObject))) as GameObject;
            subGameObject.transform.parent = transform;
            subGameObject.SetActive(false);
            // */
            gameManager = FindObjectOfType<MainGameManager>();
            
        }

        public System.Collections.IEnumerator ShowInfo() {
            yield return new WaitForSeconds(0.2f);
            gameManager.mainMenu.ShowInfoOnStart();
        }

        void OnTargetFound(TargetAbstractBehaviour behaviour) {

            //gameObj.SetActive(true);
            MyDebug.Log("TargetFound " + this.Name, true);
            /*
            if (!gameManager.wasTargetFound) {
                StartCoroutine(ShowInfo());
            }
            */
            gameManager.wasTargetFound = true;
            //gameManager.gameTypeManager.wasShowBeforeMenu = true;
            //gameManager.isARTracked = true;
            gameManager.UpdateVisibility();
        }

        void OnTargetLost(TargetAbstractBehaviour behaviour) {

            //gameObj.SetActive(false);
            //MyDebug.Log("TargetLost", true);
            //gameManager.gameTypeManager.wasShowBeforeMenu = false;
            //gameManager.isARTracked = false;
            gameManager.UpdateVisibility();
        }
    }
}
