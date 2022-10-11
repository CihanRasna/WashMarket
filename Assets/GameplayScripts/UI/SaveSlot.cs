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
        [SerializeField] private LoadGamePanel loadGamePanel;
        

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
                LoadDataIfExist(true,false);
            }
        }

        public void LoadData()
        {
            loadGamePanel.LoadData(this);
        }

        public void LoadDataIfExist(bool exist, bool isLoadScene)
        {
            if (exist)
            {
                var currencyKey = $"SaveSlot{saveIdx.ToString()}.currency";
                var dayKey = $"SaveSlot{saveIdx.ToString()}.day";
                _overwriteable = true;
                dayIdx.transform.parent.gameObject.SetActive(true);
                saveButton.gameObject.SetActive(true);
                money.text = $"Money : {ES3.Load(currencyKey)}";
                dayIdx.text = $"Day : {ES3.Load(dayKey)}";
                buttonText.text = isLoadScene ? $"Load Game" : $"Overwrite Save";
            }
            else
            {
                _overwriteable = false; 
                dayIdx.transform.parent.gameObject.SetActive(false);
                if (isLoadScene)
                {
                    buttonText.text = $"NO LOAD DATA";
                    saveButton.gameObject.SetActive(false);
                }
                else
                {
                    buttonText.text = $"Save Game";
                }
            }
        }
    }
}