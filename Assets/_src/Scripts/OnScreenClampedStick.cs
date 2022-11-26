using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace PedroAurelio.SurvivalDrive
{
    [AddComponentMenu("Input/On-Screen Clamped Stick")]
    [RequireComponent(typeof(RectTransform))]
    public class OnScreenClampedStick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private float movementRange = 50;
        [SerializeField] private bool clampHorizontally;
        [SerializeField] private bool clampVertically;
        [InputControl(layout = "Vector2"), SerializeField] private string _controlPath;

        private Vector3 _startPos;
        private Vector2 _pointerDownPos;

        private RectTransform _parentRect;
        
        protected override string controlPathInternal
        {
            get => _controlPath;
            set => _controlPath = value;
        }

        private void OnValidate()
        {
            if (!clampHorizontally && !clampVertically)
                return;

            clampHorizontally = !clampVertically;
        }

        private void Awake() => _parentRect = transform.parent.GetComponent<RectTransform>();
        private void Start() => _startPos = ((RectTransform)transform).anchoredPosition;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, eventData.position, eventData.pressEventCamera, out _pointerDownPos);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, eventData.position, eventData.pressEventCamera, out var position);
            var delta = position - _pointerDownPos;

            if (clampHorizontally)
                delta.x = 0f;

            if (clampVertically)
                delta.y = 0f;

            delta = Vector2.ClampMagnitude(delta, movementRange);
            ((RectTransform)transform).anchoredPosition = _startPos + (Vector3)delta;

            var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
            SendValueToControl(newPos);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ((RectTransform)transform).anchoredPosition = _startPos;
            SendValueToControl(Vector2.zero);
        }
    }
}
