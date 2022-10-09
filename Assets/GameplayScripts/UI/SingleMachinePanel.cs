using System.Globalization;
using DG.Tweening;
using RSNManagers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayScripts.UI
{
    public class SingleMachinePanel : Panel
    {
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
        private int _repairPrice;
        private bool _isActive;

        private void Start()
        {
            sellButton.onClick.AddListener(SellMachine);
            repairButton.onClick.AddListener(RepairMachine);
        }

        private void OnDestroy()
        {
            sellButton.onClick.RemoveListener(SellMachine);
            repairButton.onClick.AddListener(RepairMachine);
        }

        public void RepositionCurrentMachine()
        {
            var machine = currentSelectedMachine;
            var draggable = machine.gameObject.AddComponent<Draggable>();
            draggable.GetLayerMaskAndMeshData(machine.UnplaceableLayers, machine.navMeshObstacle);
            draggable.GetMachineMeshObject(machine, machine.MeshObject);
            InputManager.Instance.HasDraggableObject(machine, draggable);
            CloseMachinePanel();
        }

        public void CloseMachinePanel()
        {
            _isActive = false;
            currentSelectedMachine = null;
            rectTransform.DOAnchorPosX(400f, 1f);
        }

        public override void HidePanel()
        {
            CloseMachinePanel();
        }

        public void OpenUpMachinePanel(Machine machine)
        {
            if (!machine)
            {
                CloseMachinePanel();
                return;
            }

            _isActive = currentSelectedMachine != machine || !_isActive;

            if (_isActive)
            {
                UIManager.Instance.anyPanelActive = true;
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
            repairPriceTMP.text = $"RepairPrice :{machineRefs.repairPrice}";
            _machineMaxDurability = machineRefs.durability;
            durabilityFillImage.transform.parent.gameObject.SetActive(machineRefs.durability != 0);
            repairPriceTMP.transform.parent.gameObject.SetActive(machineRefs.durability != 0);
            capacityTMP.gameObject.SetActive(machineRefs.capacity != 0);
        }

        private void LateUpdate()
        {
            if (currentSelectedMachine)
            {
                currentSelectedMachine.RepairPricing(out var repairPrice, out var ratio, out var corruptAmount);
                var machine = currentSelectedMachine;
                var currentDurability = machine.RemainDurability.ToString("00", CultureInfo.InvariantCulture);
                var maxDurability = _machineMaxDurability.ToString("00", CultureInfo.InvariantCulture);
                var fillAmount = machine.RemainDurability / _machineMaxDurability;
                var color = Color.Lerp(Color.red, Color.green, fillAmount);
                repairPriceTMP.text = $"RepairPrice :{repairPrice.ToString("00", CultureInfo.InvariantCulture)}";
                durabilityTMP.text = $"Durability : {currentDurability} / {maxDurability}";
                durabilityFillImage.fillAmount = fillAmount;
                durabilityFillImage.color = color;
            }
        }

        private void SellMachine()
        {
            if (currentSelectedMachine)
            {
                currentSelectedMachine.Sell(out var sellPrice);
                PersistManager.Instance.Currency += sellPrice;
                CloseMachinePanel();
            }
        }

        private void RepairMachine()
        {
            if (currentSelectedMachine)
            {
                currentSelectedMachine.RepairPricing(out var repairPrice, out var ratio, out var corruptAmount);
                var persist = PersistManager.Instance;
                if (repairPrice >= persist.Currency)
                {
                    currentSelectedMachine.MachineRepaired();
                    persist.Currency -= repairPrice;
                    RefValuesFromMachine();
                }
            }
        }
    }
}