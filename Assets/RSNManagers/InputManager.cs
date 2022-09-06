using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RSNManagers
{
    public class InputManager : Singleton<InputManager> , IPointerDownHandler , IDragHandler , IPointerUpHandler
    {
        [SerializeField] private Image joystick;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("DOWN");
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("DRAG");
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("UP");
        }
    }
}