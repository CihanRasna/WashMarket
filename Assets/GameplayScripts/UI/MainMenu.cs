using RSNManagers;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayScripts.UI
{
    public class MainMenu : Panel
    {

        [SerializeField] private SaveGamePanel saveGamePanel;
        [SerializeField] private LoadGamePanel loadGamePanel;
        [SerializeField] private Panel settingPanel;

        [SerializeField] private Button resumeGameButton;
        [SerializeField] private Button saveGameButton;
        [SerializeField] private Button loadGameButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitGameButton;
        [SerializeField] private TextMeshProUGUI gameVersionText;
        

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

        private void ResumeGame()
        {
            UIManager.Instance.CloseAllPanels();
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
