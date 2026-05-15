using System;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace _.Scripts.User_Interface
{
    public class LoadingScreenViewModel : AbstractUserInterfaceViewModel
    {
        private VisualElement _container;
        
        [SerializeField] private float _containedFadeInDuration;
        [SerializeField] private Ease _containerFadeInEase;
        
        [SerializeField] private float _containedFadeOutDuration;
        [SerializeField] private Ease _containerFadeOutEase;
        
        protected override void SoftInitialize()
        {
            _container = Document.rootVisualElement.Q<VisualElement>("Container");
        }

        public void FadeIn(bool isInstant = false, bool useUnscaledTime = true, Action onComplete = null, Action<object, Tween> onUpdate = null)
        {
            if (isInstant)
            {
                _container.style.opacity = new StyleFloat(1.0f);
                
                onComplete?.Invoke();
                return;
            }
            
            Tween.VisualElementOpacity(
                    _container,
                    0.0f,
                    1.0f,
                    _containedFadeInDuration,
                    _containerFadeOutEase,
                    useUnscaledTime: useUnscaledTime)
                .OnComplete(onComplete ?? (() => { }))
                .OnUpdate(null, onUpdate ?? ((a, b) => { }));
        }

        public void FadeOut(bool isInstant = false, bool useUnscaledTime = true, Action onComplete = null, Action<object, Tween> onUpdate = null)
        {
            if (isInstant)
            {
                _container.style.opacity = new StyleFloat(0.0f);
                
                onComplete?.Invoke();
                return;
            }
            
            Tween.VisualElementOpacity(
                    _container,
                    1.0f,
                    0.0f,
                    _containedFadeOutDuration,
                    _containerFadeOutEase,
                    useUnscaledTime: useUnscaledTime)
                .OnComplete(onComplete ?? (() => { }))
                .OnUpdate(null, onUpdate ?? ((a, b) => { }));
        }
    }
}