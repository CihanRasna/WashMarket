using System;
using RSNManagers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayScripts.UI
{
    public class MainMenu : Panel
    {
        [SerializeField] private bool isGameScene;
        
        [Header("Panels")]
        [SerializeField] private LoadGamePanel loadGamePanel;
        [SerializeField,ShowIf("isGameScene")] private SaveGamePanel saveGamePanel;
        [SerializeField] private Panel settingPanel;

        [Header("Buttons")]
        [SerializeField] private Button resumeGameButton;
        [SerializeField,ShowIf("isGameScene")] private Button saveGameButton;
        [SerializeField] private Button loadGameButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitGameButton;
        [SerializeField,HideIf("isGameScene")] private TextMeshProUGUI gameVersionText;

        [SerializeField] private TextMeshProUGUI resumeGameTMP;
        [SerializeField,HideIf("isGameScene")] private TextMeshProUGUI resumeDayCountTMP;
        [SerializeField,HideIf("isGameScene")] private TextMeshProUGUI resumeMoneyCountTMP;
        
        private void Start()
        {
            if (gameVersionText != null)
            {
                gameVersionText.text = $"Version : Alpha {PlayerSettings.bundleVersion}";
            }

            resumeGameButton?.onClick.AddListener(ResumeGame);
            saveGameButton?.onClick.AddListener(OpenSavePanel);
            loadGameButton?.onClick.AddListener(OpenLoadPanel);
            settingsButton?.onClick.AddListener(OpenSettingsPanel);
            quitGameButton?.onClick.AddListener(QuitGame);

            if (!isGameScene)
            {
                CheckForAutoLoadData();
            }
        }

        private void OnDestroy()
        {
            resumeGameButton?.onClick.RemoveAllListeners();
            saveGameButton?.onClick.RemoveAllListeners();
            loadGameButton?.onClick.RemoveAllListeners();
            settingsButton?.onClick.RemoveAllListeners();
            quitGameButton?.onClick.RemoveAllListeners();
        }

        public void OpenMainMenuPanel()
        {
            gameObject.SetActive(true);
            Time.timeScale = 0f;
        }

        private void CheckForAutoLoadData()
        {
            var pathString = $"SaveSlot0.rsn";

            if (ES3.FileExists(pathString))
            {
                var currencyKey = $"SaveSlot0.currency";
                var dayKey = $"SaveSlot0.day";
                resumeGameTMP.text = "Continue";
                resumeMoneyCountTMP.text = $"Money : {ES3.Load(currencyKey)}";
                resumeDayCountTMP.text = $"Day : {ES3.Load(dayKey)}";
            }
            else
            {
                resumeDayCountTMP.gameObject.SetActive(false);
                resumeMoneyCountTMP.gameObject.SetActive(false);
                resumeGameTMP.text = "New Game";
            }
                
        }

        private void ResumeGame()
        {
            var manager = SaveLoadManager.Instance;
            var pathString = $"SaveSlot0.rsn";
            if (isGameScene)
            {
                UIManager.Instance.CloseAllPanels();
            }
            else
            {
                if (ES3.FileExists(pathString))
                {
                    manager.LoadData(0);
                }
                else
                {
                    manager.LoadData(-1);
                }
                
            }
        }

        private void OpenSavePanel()
        {
            saveGamePanel.OpenSaveGamePanel();
        }

        private void OpenLoadPanel()
        {
            loadGamePanel.OpenLoadGamePanel();
        }

        private void OpenSettingsPanel()
        {
            
        }

        private void QuitGame()
        {
            
        }
        
        
        public override void HidePanel()
        {
            gameObject.SetActive(false);
            saveGamePanel.gameObject.SetActive(false);
            loadGamePanel.gameObject.SetActive(false);
            //settingPanel.gameObject.SetActive(false);
        }
    }
}
