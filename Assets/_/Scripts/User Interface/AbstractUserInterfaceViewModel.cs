using _.Scripts.Scriptable_Objects.Global;
using _.Scripts.Services;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _.Scripts.User_Interface
{
    public abstract class AbstractUserInterfaceViewModel : MonoBehaviour
    {
        [SerializeField]                 private string     _identity;
        [SerializeField]                 private UIDocument _document;
        
        [PropertySpace] 
        [SerializeField] private bool       _autoRegisterInUserInterfaceService = true;

        public    string     Identity => _identity;
        protected UIDocument Document => _document;

        [Inject]
        private void Configure(ServiceLocator serviceLocator)
        {
            var userInterfaceService = serviceLocator.Get<UserInterfaceService>();
            
            Initialize(userInterfaceService);
        }
        
        private void Initialize(UserInterfaceService service)
        {
            if (_autoRegisterInUserInterfaceService)
                service.Add(_identity, this);
            
            SoftInitialize();
        }

        protected abstract void SoftInitialize();

        public virtual void Show()
        {
            _document.rootVisualElement.style.display = DisplayStyle.Flex;
            _document.rootVisualElement.pickingMode   = PickingMode.Position;
        }

        public virtual void Hide()
        {
            _document.rootVisualElement.style.display = DisplayStyle.None;
            _document.rootVisualElement.pickingMode = PickingMode.Ignore;
        }
    }
}