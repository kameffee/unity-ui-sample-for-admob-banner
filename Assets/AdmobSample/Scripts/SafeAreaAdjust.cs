using UnityEngine;
using UnityEngine.EventSystems;

namespace Kameffee.SafeAreaExtensions
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaAdjust : MonoBehaviour
    {
        [SerializeField]
        private bool top = true;

        [SerializeField]
        private bool bottom = true;

        [SerializeField]
        private bool left = true;

        [SerializeField]
        private bool right = true;

        private RectTransform _rectTransform;

        private DrivenRectTransformTracker _tracker = new DrivenRectTransformTracker();

        private DeviceOrientation orientation;

        private Rect lastSafeArea;

        protected void Awake()
        {
            _rectTransform = transform as RectTransform;
        }

        protected void Start()
        {
            Apply();
        }

        private void Update()
        {
            if (lastSafeArea.Equals(Screen.safeArea) && orientation == Input.deviceOrientation)
                return;

            orientation = Input.deviceOrientation;
            Apply();
        }

        private void UpdateTracker()
        {
            if (_rectTransform == null)
                _rectTransform = transform as RectTransform;

            _tracker.Clear();
            _tracker.Add(this, _rectTransform, DrivenTransformProperties.All);
            _rectTransform.anchorMin = Vector2.zero;
            _rectTransform.anchorMax = Vector2.one;
            _rectTransform.pivot = Vector2.one * 0.5f;
            _rectTransform.eulerAngles = Vector3.zero;
            _rectTransform.localScale = Vector3.one;
            _rectTransform.sizeDelta = Vector2.zero;
        }

        public void Apply()
        {
            _tracker.Clear();

            UpdateTracker();

            if (left || right)
                _rectTransform.sizeDelta = Vector2.zero;

            var safeArea = Screen.safeArea;

            var anchorMin = Vector2.zero;
            var anchorMax = Vector2.one;

            var resolition = new Resolution
            {
                width = Screen.width,
                height = Screen.height
            };
            _rectTransform.sizeDelta = Vector2.zero;
            if (right) anchorMax.x = safeArea.xMax / resolition.width;
            if (top) anchorMax.y = safeArea.yMax / resolition.height;
            if (left) anchorMin.x = safeArea.xMin / resolition.width;
            if (bottom) anchorMin.y = safeArea.yMin / resolition.height;

            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;

            lastSafeArea = safeArea;
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            UpdateTracker();
            // Apply();
        }

        protected void Reset()
        {
            UpdateTracker();
        }
#endif
    }
}