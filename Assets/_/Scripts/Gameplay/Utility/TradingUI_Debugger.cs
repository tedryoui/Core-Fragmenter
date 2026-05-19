using _.Scripts.Services;
using _.Scripts.User_Interface;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _.Scripts.Gameplay.Utility
{
    public class TradingUI_Debugger : MonoBehaviour
    {
        [Button]
        private void OpenTradeUI()
        {
            var lifetimeScope  = GameObject.FindAnyObjectByType<LifetimeScope>();
            var objectResolver = lifetimeScope.Container;
            var uiService      = objectResolver.Resolve<ServiceLocator>().Get<UserInterfaceService>();

            uiService.Get<TradingWindowViewModel>().Show();
        }
    }
}