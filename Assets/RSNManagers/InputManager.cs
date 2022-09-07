using GameplayScripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RSNManagers
{
    public class InputManager : Singleton<InputManager>, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private Joystick joystick;
        [SerializeField] private Player currentPlayer;

        private bool _hasInputValue;
        private bool _hasMover;

        protected override void Start()
        {
            base.Start();
            currentPlayer = GameManager.Instance.currentPlayer;
            _hasMover = currentPlayer;
        }

        private void Update()
        {
            if (_hasInputValue && _hasMover)
            {
                currentPlayer.Move(joystick.Direction);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            joystick.OnPointerDown(eventData);
            _hasInputValue = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            joystick.OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            joystick.OnPointerUp(eventData);
            _hasInputValue = false;
        }
    }
}