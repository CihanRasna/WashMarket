using GameplayScripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RSNManagers
{
    public class InputManager : Singleton<InputManager>, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private Joystick joystick;
        [SerializeField] private Player currentPlayer;
        [SerializeField] private LayerMask rayCastLayers;
        

        private bool _hasInputValue;
        private bool _hasMover;
        private bool _firstTouch;
        private Camera _camera;

        protected override void Start()
        {
            base.Start();
            _camera = Camera.main;
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
            if (!_firstTouch)
            {
                _firstTouch = true;
                GameManager.Instance.StartGame();
            }

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
        
            if (Physics.Raycast(ray, out var hit,1000f,rayCastLayers)) 
            {
                Debug.Log(hit.collider.TryGetComponent(out Machine machine));
                return;
            }
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