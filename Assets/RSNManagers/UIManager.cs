using System;
using GameplayScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace RSNManagers
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private MachineList machineList;
        [SerializeField] private TextMeshProUGUI currencyText;

        protected override void Start()
        {
            base.Start();
            PersistManager.Instance.CurrencyChangedEvent += UpdateCurrency;
            UpdateCurrency(PersistManager.Instance.Currency);
        }

        private void UpdateCurrency(int currency)
        {
            currencyText.text = currency.ToString();
        }


        public void OpenFailedPanel()
        {
            Debug.Log("failed");
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                machineList.OpenUpList();
            }
        }
    }
}