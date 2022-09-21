using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameplayScripts.UI
{
    public class MachineListView : MonoBehaviour , IBeginDragHandler
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private List<Button> machineButtons;

        private void Start()
        {
            scrollRect.onValueChanged.AddListener(Scrolling);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            scrollRect.OnBeginDrag(eventData);
            Debug.Log("ZA");
            //scrollRect.OnBeginDrag();
        }

        private void Scrolling(Vector2 value)
        {
            var velocity = scrollRect.velocity.magnitude;
            if (value.magnitude >= 1f)
            {
                scrollRect.StopMovement();
                velocity = 0f;
            }

            EnableDisableButtons(velocity <= 10f);
        }

        private void EnableDisableButtons(bool val)
        {
            foreach (var button in machineButtons)
            {
                button.interactable = val;
            }
        }
    }
}
