using System;
using GameplayScripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RSNManagers
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private MachineList machineList;
        [SerializeField] private SingleMachinePanel singleMachinePanel;
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

        public void PurchaseButtonIsPressed(bool forceDeactivate = false)
        {
            machineList.OpenUpList(forceDeactivate);
        }

        public void SingleMachineSelected(Machine machine)
        {
            singleMachinePanel.OpenUpMachinePanel(machine);
        }


        public void OpenFailedPanel()
        {
            Debug.Log("failed");
        }
    }
}