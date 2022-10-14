using System;
using System.Collections.Generic;
using RSNManagers;
using UnityEngine;

namespace GameplayScripts.UI
{
    public class LoadGamePanel : Panel
    {
        [SerializeField] private List<SaveSlot> loadSlots;

        private void Start()
        {
            LoadDataIfExist();
            SaveLoadManager.Instance.GameSavedEvent += GameSaved;
        }

        private void OnApplicationQuit()
        {
            SaveLoadManager.Instance.GameSavedEvent -= GameSaved;
        }

        private void GameSaved()
        {
            LoadDataIfExist();
        }

        public void OpenLoadGamePanel()
        {
            gameObject.SetActive(true);
        }

        public void LoadData(SaveSlot slot)
        {
            var manager = SaveLoadManager.Instance;
            manager.LoadData(slot.SaveIdx);
            LoadDataIfExist();
        }

        public override void HidePanel()
        {
            gameObject.SetActive(false);
        }

        private void LoadDataIfExist()
        {
            for (int i = 1; i < loadSlots.Count + 1; i++)
            {
                var pathString = $"SaveSlot{i.ToString()}.rsn";
                var key = $"SaveSlot{i.ToString()}";
                var settings = new ES3Settings(ES3.EncryptionType.AES, "saveCrypt")
                {
                    compressionType = ES3.CompressionType.Gzip,
                    location = ES3.Location.File,
                    path = pathString
                };
                loadSlots[i - 1].LoadDataIfExist(ES3.KeyExists(key, settings), true);
            }
        }
    }
}