using System;
using DG.Tweening;
using GameplayScripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RSNManagers
{
    public class InputManager : Singleton<InputManager>, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private Joystick joystick;
        [SerializeField] private Player currentPlayer;
        [SerializeField] private LayerMask placementLayers;
        [SerializeField] private LayerMask rayCastLayers;

        private Machine _currentMachine = null;
        private Draggable _currentDraggable = null;

        private bool _hasInputValue;
        private bool _hasMover;
        private bool _firstTouch;
        private Camera _camera;
        
        private Vector3 _screenPoint;
        private Vector3 _offset;
        private float _yPos;

        protected override void Start()
        {
            base.Start();
            _camera = Camera.main;
            currentPlayer = GameManager.Instance.currentPlayer;
            _hasMover = currentPlayer;
        }

        public void HasDraggableObject(Machine machine,Draggable draggable)
        {
            _currentMachine = machine;
            _currentDraggable = draggable;
        }

        private void Update()
        {
            if (_hasInputValue && _hasMover)
            {
                currentPlayer.Move(joystick.Direction);
            }
        }

        private void FixedUpdate()
        {
            if (_currentDraggable)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    _currentDraggable.transform.DOLocalRotate(new Vector3(0, 90f, 0), 0.3f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutQuad);
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    _currentDraggable.transform.DORotate(new Vector3(0, -90f, 0), 0.3f,RotateMode.LocalAxisAdd).SetEase(Ease.InOutQuad);
                }
                if (RaycastFromMouse(out var hit, placementLayers))
                {
                    var pos = hit.point;
                    var roundedPos = new Vector3
                    {
                        x = Mathf.RoundToInt(pos.x),
                        y = Mathf.RoundToInt(pos.y),
                        z = Mathf.RoundToInt(pos.z)
                    };
                    var selfPos = _currentDraggable.transform.position;

                    _currentDraggable.transform.position = new Vector3(roundedPos.x,selfPos.y,roundedPos.z);
                }
            }
        }

        private bool RaycastFromMouse(out RaycastHit h, LayerMask layer)
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out h, Mathf.Infinity, layer);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_firstTouch)
            {
                _firstTouch = true;
                GameManager.Instance.StartGame();
            }

            if (_currentDraggable && _currentDraggable.CanPlace)
            {
                _currentMachine.navMeshObstacle.enabled = true;
                _currentDraggable.Placed();
                _currentDraggable = null;
                _currentMachine = null;
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
            if (_currentDraggable)
            {
                
            }
            else
            {
                joystick.OnDrag(eventData);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            joystick.OnPointerUp(eventData);
            if (_hasInputValue && _hasMover)
            {
                currentPlayer.Stop();
            }
            _hasInputValue = false;
        }
    }
}