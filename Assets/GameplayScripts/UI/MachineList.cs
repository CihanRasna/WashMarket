using System.Collections.Generic;
using DG.Tweening;
using RSNManagers;
using TMPro;
using UnityEngine;

namespace GameplayScripts.UI
{
    public class MachineList : Panel
    {
        [SerializeField] private List<Machine> machineInfos;

        [SerializeField] private TextMeshProUGUI machineName;
        [SerializeField] private TextMeshProUGUI machineLevel;
        [SerializeField] private TextMeshProUGUI singleWorkTime;
        [SerializeField] private TextMeshProUGUI durability;
        [SerializeField] private TextMeshProUGUI capacity;
        [SerializeField] private TextMeshProUGUI consumption;
        [SerializeField] private TextMeshProUGUI usingPrice;
        [SerializeField] private TextMeshProUGUI machinePrice;

        [SerializeField] private RectTransform infoPanel;
        [SerializeField] private CanvasGroup infoPanelCanvasGroup;

        private int _index = -1;
        public bool isActive = false;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void OpenUpList(bool forceDeactivate = false)
        {
            DOTween.Kill(1);

            isActive = !forceDeactivate && !isActive;

            rectTransform.DOAnchorPosX(isActive ? 100f : -400f, 1f);
        }

        public override void HidePanel()
        {
            OpenUpList(true);
        }

        public void SpawnMachine()
        {
            var currency = PersistManager.Instance.Currency;
            if (_index >= 0)
            {
                var desiredMachine = machineInfos[_index];
                if (currency >= desiredMachine.BuyPrice)
                {
                    OpenUpList();
                    //PersistManager.Instance.Currency -= desiredMachine.BuyPrice;
                    var machine = Instantiate(desiredMachine);
                    var draggable = machine.gameObject.AddComponent<Draggable>();
                    draggable.GetLayerMaskAndMeshData(machine.UnplaceableLayers,machine.navMeshObstacle, desiredMachine.BuyPrice);
                    draggable.GetMachineMeshObject(machine,machine.MeshObject);
                    InputManager.Instance.HasDraggableObject(machine,draggable);
                }
            }
        }

        private void RefValuesFromMachine(Machine machine)
        {
            var machineRefs = machine.RefValuesForUI();
            machineName.text = machineRefs.machineName;
            machineLevel.text = $"Level : {machineRefs.currentLevel}";
            singleWorkTime.text = $"Work Time : {machineRefs.singleWorkTime}";
            durability.text = $"Durability : {machineRefs.durability}";
            usingPrice.text = $"Profit : {machineRefs.usingPrice}";
            if (machineRefs.usingPrice == 0)
            {
                usingPrice.text = string.Empty;
            }
            machinePrice.text = $"Price : {machineRefs.buyPrice}";
        }

        public void OpenUpTooltipPanel(int idx)
        {
            _index = idx;
            var desiredMachine = machineInfos[idx];
            RefValuesFromMachine(desiredMachine);
            infoPanel.DOAnchorPosX(350, 1f);
            var currentAlpha = infoPanelCanvasGroup.alpha;
            DOVirtual.Float(currentAlpha, 1f, 1f, value => infoPanelCanvasGroup.alpha = value);
        }

        public void ExitMouse()
        {
            _index = -1;
            infoPanel.DOAnchorPosX(0, 1f);
            var currentAlpha = infoPanelCanvasGroup.alpha;
            DOVirtual.Float(currentAlpha, 0f, 1f, value => infoPanelCanvasGroup.alpha = value);
        }
    }
}