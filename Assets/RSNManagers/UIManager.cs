using System.Collections.Generic;
using DG.Tweening;
using GameplayScripts.Machines;
using GameplayScripts.UI;
using TMPro;
using UnityEngine;

namespace RSNManagers
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private MachineList machineList;
        [SerializeField] private SingleMachinePanel singleMachinePanel;
        [SerializeField] private MainMenu mainMenuPanel;
        [SerializeField] private AllMachinesRect allMachinesMenu;
        [SerializeField] private TextMeshProUGUI currencyText;

        [SerializeField] private List<Panel> uiPanels;

        public bool anyPanelActive;
        
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

        public void OpenSingleMachinePanel(Machine machine)
        {
            singleMachinePanel.OpenSingleMachinePanel(machine);
        }

        public void HideAllPanels()
        {
            foreach (var panel in uiPanels)
            {
                panel.HidePanel();
            }

            anyPanelActive = false;
            if (Time.timeScale == 0f)
            {
                Time.timeScale = 1f;
            }
        }

        public void OpenMainMenu()
        {
            anyPanelActive = true;
            mainMenuPanel.OpenMainMenuPanel();
        }

        public void OpenAllMachinesList()
        {
            allMachinesMenu.OpenAllMachinesPanel();
        }


        public void OpenFailedPanel()
        {
            anyPanelActive = true;
            Debug.Log("failed");
        }
    }
}