using System;
using System.Collections.Generic;
using DG.Tweening;
using RSNManagers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameplayScripts.UI
{
    public class SaveGamePanel : Panel
    {
        [SerializeField] private List<SaveSlot> saveSlots;

        [SerializeField] private CanvasGroup overwritePanel;
        [SerializeField] private Button overwriteButton;
        [SerializeField] private Button cancelButton;
        private int _selectedSlotIdx;

        public void OpenSaveGamePanel()
        {
            gameObject.SetActive(true);
        }

        private void Start()
        {
            LoadDataIfExist();
            overwriteButton.onClick.AddListener(OverwriteData);
            cancelButton.onClick.AddListener(CloseOverwritePanel);
        }

        private void OnApplicationQuit()
        {
            overwriteButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
        }

        public void OpenOverwritePanel(SaveSlot slot)
        {
            DOTween.Kill(9999);
            overwritePanel.gameObject.SetActive(true);
            _selectedSlotIdx = slot.SaveIdx;
            DOVirtual.Float(0f, 1f, 0.5f, OnVirtualUpdate).OnComplete((() =>
            {
                overwritePanel.interactable = true;
                overwritePanel.blocksRaycasts = true;
            })).SetId(9999);
        }

        private void CloseOverwritePanel()
        {
            DOTween.Kill(9999);
            DOVirtual.Float(1f, 0f, 0.5f, OnVirtualUpdate).OnComplete((() =>
            {
                overwritePanel.interactable = false;
                overwritePanel.blocksRaycasts = false;
                overwritePanel.gameObject.SetActive(false);
            })).SetId(9999);
        }

        public void OverwriteData()
        {
            var manager = SaveLoadManager.Instance;
            manager.SaveData(_selectedSlotIdx);
            CloseOverwritePanel();
            LoadDataIfExist();
        }

        private void OnVirtualUpdate(float value)
        {
            overwritePanel.alpha = value;
        }

        private void LoadDataIfExist()
        {
            for (int i = 1; i < saveSlots.Count + 1; i++)
            {
                var pathString = $"SaveSlot{i.ToString()}.rsn";
                var key = $"{i.ToString()}.rsn";
                var settings = new ES3Settings(ES3.EncryptionType.None, "myPassword")
                {
                    location = ES3.Location.File,
                    path = pathString
                };
                if (ES3.KeyExists(key, settings))
                {
                    Debug.Log("EXIST");
                    saveSlots[i - 1].LoadDataIfExist(true);
                }
                else
                {
                    saveSlots[i - 1].LoadDataIfExist(false);
                }
            }
        }

        public override void HidePanel()
        {
            gameObject.SetActive(false);
        }
    }
}