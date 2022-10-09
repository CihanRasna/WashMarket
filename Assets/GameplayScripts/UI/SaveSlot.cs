using RSNManagers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayScripts.UI
{
    public class SaveSlot : MonoBehaviour
    {
        [SerializeField] private int saveIdx;
        public int SaveIdx => saveIdx;
        [SerializeField] private TextMeshProUGUI slotIdx;
        [SerializeField] private TextMeshProUGUI dayIdx;
        [SerializeField] private TextMeshProUGUI money;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private SaveGamePanel saveGamePanel;

        private bool _overwriteable;

        [SerializeField] private Button saveButton;

        private void Start()
        {
            saveGamePanel = GetComponentInParent<SaveGamePanel>();
            //saveButton.onClick.AddListener(SaveData);
        }

        public void SaveData()
        {
            var saveManager = SaveLoadManager.Instance;
           
            if (_overwriteable)
            {
                saveGamePanel.OpenOverwritePanel(this);
            }
            else
            {
                saveManager.SaveData(saveIdx);
                LoadDataIfExist(true);
            }
        }

        public void LoadDataIfExist(bool exist)
        {
            if (exist)
            {
                _overwriteable = true;
                dayIdx.transform.parent.gameObject.SetActive(true);
                dayIdx.text = $"Day : {PersistManager.Instance.PassedDayCount.ToString()}";
                money.text = $"Day : {PersistManager.Instance.Currency.ToString()}";
                buttonText.text = $"Overwrite Save";
            }
            else
            {
                _overwriteable = false; 
                dayIdx.transform.parent.gameObject.SetActive(false);
                buttonText.text = $"Save Game";
            }
        }
    }
}