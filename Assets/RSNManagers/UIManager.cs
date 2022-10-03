using System;
using DG.Tweening;
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
        private int _lastKnownCurrency;

        protected override void Start()
        {
            base.Start();
            PersistManager.Instance.CurrencyChangedEvent += UpdateCurrency;
            UpdateCurrency(_lastKnownCurrency = PersistManager.Instance.Currency);
        }

        private void UpdateCurrency(int currency)
        {
            DOTween.To(() => _lastKnownCurrency, x => _lastKnownCurrency = x, currency, 0.5f)
                .OnUpdate((() => currencyText.text = _lastKnownCurrency.ToString()))
                .OnComplete((() => _lastKnownCurrency = currency));
            //currencyText.DOText(currency.ToString(), 0.2f, true, ScrambleMode.Numerals);
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