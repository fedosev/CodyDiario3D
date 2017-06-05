/**
* Copyright (c) 2015-2016 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
* EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
* and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
*/

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using EasyAR;

namespace EasyARSample
{
    public class ImageTargetManager : MonoBehaviour
    {
        private Dictionary<string, DynamicImageTagetBehaviour> imageTargetDic = new Dictionary<string, DynamicImageTagetBehaviour>();
        private FilesManager pathManager;

        void Start()
        {
            if (!pathManager)
                pathManager = FindObjectOfType<FilesManager>();
        }

        void Update()
        {
            var imageTargetName_FileDic = pathManager.GetDirectoryName_FileDic();
            foreach (var obj in imageTargetName_FileDic.Where(obj => !imageTargetDic.ContainsKey(obj.Key)))
            {
                GameObject imageTarget = new GameObject(obj.Key);
                var behaviour = imageTarget.AddComponent<DynamicImageTagetBehaviour>();
                behaviour.Name = obj.Key;
                behaviour.Path = obj.Value.Replace(@"\", "/");
                behaviour.Storage = StorageType.Absolute;
                behaviour.Bind(ARBuilder.Instance.TrackerBehaviours[0]);
                imageTargetDic.Add(obj.Key, behaviour);
            }
        }

        public void ClearAllTarget()
        {
            foreach (var obj in imageTargetDic)
                Destroy(obj.Value.gameObject);
            imageTargetDic = new Dictionary<string, DynamicImageTagetBehaviour>();
        }
    }
}
