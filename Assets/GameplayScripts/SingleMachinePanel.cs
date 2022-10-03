using System;
using System.Globalization;
using DG.Tweening;
using RSNManagers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayScripts
{
    public class SingleMachinePanel : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        
        [SerializeField] private Machine currentSelectedMachine;
        [SerializeField] private TextMeshProUGUI machineNameTMP;
        [SerializeField] private TextMeshProUGUI machineLevelTMP;
        [SerializeField] private TextMeshProUGUI singleWorkTimeTMP;
        [SerializeField] private TextMeshProUGUI durabilityTMP;
        [SerializeField] private Image durabilityFillImage;
        [SerializeField] private TextMeshProUGUI capacityTMP;
        [SerializeField] private TextMeshProUGUI totalGainTMP;
        [SerializeField] private TextMeshProUGUI sellPriceTMP;
        [SerializeField] private TextMeshProUGUI repairPriceTMP;
        [SerializeField] private Button sellButton;
        [SerializeField] private Button repairButton;

        private float _machineMaxDurability;
        private bool _isActive;

        public void RepositionCurrentMachine()
        {
            var machine = currentSelectedMachine;
            var draggable = machine.gameObject.AddComponent<Draggable>();
            draggable.GetLayerMaskAndMeshData(machine.UnplaceableLayers,machine.navMeshObstacle);
            draggable.GetMachineMeshObject(machine.MeshObject);
            InputManager.Instance.HasDraggableObject(machine,draggable);
            OpenUpMachinePanel();
        }

        public void OpenUpMachinePanel()
        {
            _isActive = false;
            currentSelectedMachine = null;
            rectTransform.DOAnchorPosX(400f, 1f);
        }

        public void OpenUpMachinePanel(Machine machine)
        {
            if (!machine)
            {
                OpenUpMachinePanel();
                return;
            }

            _isActive = currentSelectedMachine != machine || !_isActive;

            if (_isActive)
            {
                currentSelectedMachine = machine;
                RefValuesFromMachine();
                rectTransform.DOAnchorPosX(-100f, 1f);
            }
            else
            {
                currentSelectedMachine = null;
                rectTransform.DOAnchorPosX(400f, 1f);
            }
        }


        private void RefValuesFromMachine()
        {
            var machineRefs = currentSelectedMachine.RefValuesForUI();
            machineNameTMP.text = machineRefs.machineName;
            machineLevelTMP.text = $"Level : {machineRefs.currentLevel}";
            singleWorkTimeTMP.text = $"Work Time : {machineRefs.singleWorkTime}";
            capacityTMP.text = $"Capacity : {machineRefs.capacity}";
            sellPriceTMP.text = $"SellPrice : {machineRefs.sellPrice}";
            totalGainTMP.text = $"Total Gain : {machineRefs.totalGain}";
            _machineMaxDurability = machineRefs.durability;
        }

        private void LateUpdate()
        {
            if (currentSelectedMachine)
            {
                var machine = currentSelectedMachine;
                var currentDurability = machine.RemainDurability.ToString(CultureInfo.InvariantCulture);
                var maxDurability = _machineMaxDurability.ToString(CultureInfo.InvariantCulture);
                var fillAmount = machine.RemainDurability / _machineMaxDurability;
                var color = Color.Lerp(Color.red, Color.green, fillAmount);
                durabilityTMP.text = $"Durability : {currentDurability} / {maxDurability}";
                durabilityFillImage.fillAmount = fillAmount;
                durabilityFillImage.color = color;
            }
        }

        public void SellMachine()
        {
            
        }

        public void RepairMachine()
        {
            
        }
    }
}
