using System;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using RSNManagers;
using TMPro;
using UnityEngine;

namespace GameplayScripts
{
    public class MachineList : MonoBehaviour
    {
        private RectTransform _rectTransform;
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
            _rectTransform = GetComponent<RectTransform>();
        }

        public void OpenUpList()
        {
            isActive = !isActive;
            if (isActive)
            {
                _rectTransform.DOAnchorPosX(100f, 1f);
            }
            else
            {
                _rectTransform.DOAnchorPosX(-400f, 1f);
            }
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
                    PersistManager.Instance.Currency -= desiredMachine.BuyPrice;
                    var machine = Instantiate(desiredMachine,Vector3.zero, Quaternion.identity);
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