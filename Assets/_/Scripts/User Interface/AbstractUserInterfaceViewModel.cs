using _.Scripts.Scriptable_Objects.Global;
using _.Scripts.Services;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _.Scripts.User_Interface
{
    public abstract class AbstractUserInterfaceViewModel : MonoBehaviour
    {
        [SerializeField] private string _identity;
        [SerializeField] private UIDocument _document;

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
            service.Add(_identity, this);
            
            SoftInitialize();
        }

        protected abstract void SoftInitialize();
    }
}