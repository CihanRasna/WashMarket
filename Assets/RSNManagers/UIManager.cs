using System;
using DG.Tweening;
using UnityEngine;

namespace RSNManagers
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private RectTransform machineListView;
        
        public void OpenFailedPanel()
        {
            Debug.Log("failed");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                machineListView.DOAnchorPosX(100, 1f);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                machineListView.DOAnchorPosX(-500, 1f);
            }
        }
    }
}