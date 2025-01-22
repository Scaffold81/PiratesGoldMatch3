using DG.Tweening;
using UnityEngine;
using Game.Enums;

namespace Game.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanelStateController : MonoBehaviour
    {
        [SerializeField]
        private EventNames _uIPanelName;
        private CanvasGroup _canvasGroup;
        private float _duration = 0.5f;

        public EventNames UIPanelName { get => _uIPanelName; set => _uIPanelName = value; }
        public bool IsActive { get; set; }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show()
        {
            var startValue = 0f;
            var endValue = 1f;
            IsActive = true;
            DOTween.To(() => startValue, x => startValue = x, endValue, _duration)
                .OnUpdate(() =>
                {
                    _canvasGroup.alpha = startValue; // Обновление прозрачности в процессе анимации
                })
                .OnComplete(() =>
                {
                   
                    _canvasGroup.blocksRaycasts = true;
                    _canvasGroup.interactable = true;
                });

        }

        public void Hide()
        {
            var startValue = 1f;
            var endValue = 0f;
            IsActive = false;
            DOTween.To(() => startValue, x => startValue = x, endValue, _duration)
               .OnUpdate(() =>
               {
                   _canvasGroup.alpha = startValue;
               })
               .OnComplete(() =>
               {
                   _canvasGroup.blocksRaycasts = false;
                   _canvasGroup.interactable = false;
               });
        }
    }
}
