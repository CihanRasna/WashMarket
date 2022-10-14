using System;
using System.Globalization;
using GameplayScripts.Machines;
using RSNManagers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayScripts.UI
{
    public class ListViewMachinePanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI machineName;
        [SerializeField] private Image machineIcon;
        [SerializeField] private Image durabilityImage;
        [SerializeField] private TextMeshProUGUI sellPriceTMP;
        [SerializeField] private TextMeshProUGUI repairPriceTMP;

        [SerializeField] private Button selectButton;
        [SerializeField] private Button repairButton;
        [SerializeField] private Button sellButton;

        private Machine _currentMachine;
        private float _machineMaxDurability;

        [SerializeField] private AllMachinesRect rectController;

        public void GetCurrentMachineData(Machine currentMachine, AllMachinesRect allMachinesRect = null)
        {
            if (allMachinesRect) rectController = allMachinesRect;
            
            _currentMachine = currentMachine;
            var machineRefs = currentMachine.RefValuesForUI();
            machineName.text = machineRefs.machineName;
            machineIcon.sprite = null;
            durabilityImage.transform.parent.gameObject.SetActive(machineRefs.durability != 0);
            durabilityImage.fillAmount = currentMachine.RemainDurability / machineRefs.durability;
            sellPriceTMP.text = $"Sell Price : {machineRefs.sellPrice.ToString()}";

            if (machineRefs.repairPrice > 0)
            {
                repairPriceTMP.gameObject.SetActive(true);
                repairPriceTMP.text = $"Repair Price : {machineRefs.repairPrice.ToString()}";
            }
            else
            {
                repairPriceTMP.gameObject.SetActive(false);
            }
            
            sellButton.onClick.AddListener(SellMachineButtonTapped);
            repairButton.onClick.AddListener(RepairMachineButtonTapped);
            selectButton.onClick.AddListener(SelectMachineButtonTapped);
            _machineMaxDurability = machineRefs.durability;
        }

        public void UpdateCurrentMachineData()
        {
            if (_currentMachine)
            {
                _currentMachine.RepairPricing(out var repairPrice, out var ratio,out var sellPricing);
                var machine = _currentMachine;
                var fillAmount = machine.RemainDurability / _machineMaxDurability;
                var color = Color.Lerp(Color.red, Color.green, fillAmount);
                durabilityImage.fillAmount = fillAmount;
                durabilityImage.color = color;
                repairPriceTMP.text = machine.Repairing ? $"Repairing..." :
                    machine.Filled ? $"Working" :
                    $"RepairPrice : {repairPrice.ToString()}";

                sellPriceTMP.text = $"Sell : {sellPricing.ToString()}$";
                
                repairButton.interactable = ratio < 1f;
            }
        }

        private void OnDestroy()
        {
            selectButton.onClick.RemoveAllListeners();
            repairButton.onClick.RemoveAllListeners();
            sellButton.onClick.RemoveAllListeners();
        }

        private void SelectMachineButtonTapped()
        {
            if (!_currentMachine) return;
            var manager = UIManager.Instance;
            rectController.HidePanel();
            manager.OpenSingleMachinePanel(_currentMachine);
        }
        
        private void SellMachineButtonTapped()
        {
            if (!_currentMachine) return;
            _currentMachine.Sell(out var sellPrice);
            PersistManager.Instance.Currency += sellPrice;
            rectController.UpdateAllMachinesData(this);
        }

        private void RepairMachineButtonTapped()
        {
            if (!_currentMachine) return;
            _currentMachine.RepairPricing(out var repairPrice, out var ratio,out var sellPricing);
            var persist = PersistManager.Instance;
            if (repairPrice > persist.Currency) return;
            _currentMachine.MachineRepairing();
            persist.Currency -= repairPrice;
            GetCurrentMachineData(_currentMachine);
        }
    }
}
