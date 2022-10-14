using System;
using System.Collections.Generic;
using DG.Tweening;
using RSNManagers;
using UnityEngine;

namespace GameplayScripts.UI
{
    public class AllMachinesRect : Panel
    {
        [SerializeField] private ListViewMachinePanel panelPrefab;
        [SerializeField] private Transform contextMenu;

        [SerializeField] private List<ListViewMachinePanel> currentMachinesInfoPanels;
        private bool _isActive = false;

        public void OpenAllMachinesPanel()
        {
            _isActive = !_isActive;

            if (_isActive)
            {
                gameObject.SetActive(_isActive);
                var manager = GameManager.Instance;
                var allMachines = manager.allMachines;
                var totalMachineCount = allMachines.Count;
                var currentPanelCount = currentMachinesInfoPanels.Count;
                var diff = totalMachineCount - currentPanelCount;
                for (var i = 0; i < diff; i++)
                {
                    var panel = Instantiate(panelPrefab, contextMenu);
                    currentMachinesInfoPanels.Add(panel);
                    panel.GetCurrentMachineData(allMachines[i], this);
                }
            }
            else
            {
                HidePanel();
            }
        }

        public void UpdateAllMachinesData(ListViewMachinePanel panel)
        {
            currentMachinesInfoPanels.Remove(panel);
            panel.transform.DOScale(0f, 0.5f).OnComplete(() => Destroy(panel.gameObject));
        }

        private void LateUpdate()
        {
            if (_isActive)
            {
                foreach (var panel in currentMachinesInfoPanels)
                {
                    panel.UpdateCurrentMachineData();
                }
            }
        }

        public override void HidePanel()
        {
            gameObject.SetActive(false);
        }
    }
}