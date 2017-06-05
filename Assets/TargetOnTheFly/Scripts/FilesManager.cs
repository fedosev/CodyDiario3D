/**
* Copyright (c) 2015-2016 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
* EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
* and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
*/

using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;


namespace EasyARSample
{
    public class FilesManager : MonoBehaviour
    {
        private string MarksDirectory;
        private bool isWriting;
        private TargetOnTheFly ui;

        void Awake()
        {
            ui = FindObjectOfType<TargetOnTheFly>();
            MarksDirectory = Application.persistentDataPath;
            Debug.Log("MarkPath:" + Application.persistentDataPath);
        }

        public void StartTakePhoto()
        {
            if (!Directory.Exists(MarksDirectory))
                Directory.CreateDirectory(MarksDirectory);
            if (!isWriting)
                StartCoroutine(ImageCreate());
        }

        IEnumerator ImageCreate()
        {
            isWriting = true;
            yield return new WaitForEndOfFrame();

            Texture2D photo = new Texture2D((int)(Screen.width * ui.width), (int)(Screen.height * ui.height), TextureFormat.RGB24, false);
            photo.ReadPixels(new Rect(Screen.width * (1 - ui.width) / 2, Screen.height * (1 - ui.height) / 2, Screen.width * ui.width, Screen.height * ui.height), 0, 0, false);
            photo.Apply();

            byte[] data = photo.EncodeToJPG(80);
            DestroyImmediate(photo);
            photo = null;

            string photoPath = Path.Combine(MarksDirectory, "photo" + DateTime.Now.Ticks + UnityEngine.Random.Range(-1f, 1f) + ".jpg");

            using (FileStream file = File.Open(photoPath, FileMode.Create))
            {
                file.BeginWrite(data, 0, data.Length, new AsyncCallback(endWriter), file);
            }
            ui.UICanvas.active = true;
            ui.isShowUI = true;
            ui.game.active = true;
        }

        void endWriter(IAsyncResult end)
        {
            using (FileStream endwriter = (FileStream)end.AsyncState)
            {
                endwriter.EndWrite(end);
                isWriting = false;
                ui.StartShowMessage = true;
            }
        }

        public Dictionary<string, string> GetDirectoryName_FileDic()
        {
            if (!Directory.Exists(MarksDirectory))
                return new Dictionary<string, string>();
            return GetAllImagesFiles(MarksDirectory);
        }

        private Dictionary<string, string> GetAllImagesFiles(string path)
        {
            Dictionary<string, string> imgefilesDic = new Dictionary<string, string>();
            foreach (var file in Directory.GetFiles(path))
            {
                if (Path.GetExtension(file) == ".jpg" || Path.GetExtension(file) == ".bmp" || Path.GetExtension(file) == ".png")
                    imgefilesDic.Add(Path.GetFileNameWithoutExtension(file), file);
            }
            return imgefilesDic;
        }

        public void ClearTexture()
        {
            Dictionary<string, string> imageFileDic = GetAllImagesFiles(MarksDirectory);
            foreach (var path in imageFileDic)
                File.Delete(path.Value);
        }
    }
}
