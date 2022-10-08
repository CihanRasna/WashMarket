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

        private UIManager UIManager;

        protected override void Start()
        {
            base.Start();
            _camera = Camera.main;
            currentPlayer = GameManager.Instance.currentPlayer;
            UIManager = UIManager.Instance;
            _hasMover = currentPlayer;
        }

        public void HasDraggableObject(Machine machine, Draggable draggable)
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

            if (!_currentDraggable && Input.GetKeyDown(KeyCode.B))
            {
                UIManager.SingleMachineSelected(null);
                UIManager.PurchaseButtonIsPressed();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_currentDraggable)
                {
                    Destroy(_currentDraggable.gameObject);
                    HasDraggableObject(null,null);
                }
                else
                {
                    UIManager.CloseAllPanels();
                }
            }
            
            if (_currentDraggable)
            {
                if (!_currentDraggable.isRotating && Input.GetKeyDown(KeyCode.Q))
                {
                    _currentDraggable.isRotating = true;
                    _currentDraggable.transform.DOLocalRotate(new Vector3(0, 90f, 0), 0.3f, RotateMode.LocalAxisAdd)
                        .SetEase(Ease.InOutQuad).OnComplete((() => _currentDraggable.isRotating = false));
                }

                if (!_currentDraggable.isRotating && Input.GetKeyDown(KeyCode.E))
                {
                    _currentDraggable.isRotating = true;
                    _currentDraggable.transform.DORotate(new Vector3(0, -90f, 0), 0.3f, RotateMode.LocalAxisAdd)
                        .SetEase(Ease.InOutQuad).OnComplete((() => _currentDraggable.isRotating = false));
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
                    var draggableTransform = _currentDraggable.transform;
                    var selfPos = draggableTransform.position;

                    draggableTransform.position = new Vector3(roundedPos.x, selfPos.y, roundedPos.z);
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

            if (_currentDraggable)
            {
                if (_currentDraggable.CanPlace)
                {
                    _currentMachine.navMeshObstacle.enabled = true;
                    _currentDraggable.Placed();
                    _currentDraggable = null;
                    _currentMachine = null;
                }
            }
            else
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit, 1000f, rayCastLayers))
                {
                    hit.collider.TryGetComponent(out Machine machine);
                    UIManager.PurchaseButtonIsPressed(true);
                    UIManager.SingleMachineSelected(machine);
                    return;
                }
                else
                {
                    UIManager.CloseAllPanels();
                }
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