using System.Collections.Generic;
using _.Scripts.Gameplay.Entity.Concrete.Trade_Fabric;
using _.Scripts.Gameplay.Player;
using _.Scripts.Services;
using _.Scripts.User_Interface;
using UnityEngine;
using VContainer;

namespace _.Scripts.Gameplay.Entity.Concrete
{
    public class TradeFabricEntity : MonoEntity
    {
#region VContainer

        [Inject] private PlayerProfile  _playerProfile;
        [Inject] private ServiceLocator _serviceLocator;

#endregion
        
        public override          string Identity => _identity;

        public override List<AbstractState> PossibleStates => new List<AbstractState>()
        {
            new TradeFabricIdleState(this),
            new TradeFabricOpenedState(this)
        };

        [SerializeField] private string _identity;
        
        [SerializeField] private WorldUtilityWindowViewModel _worldUtilityWindowViewModel;
        public WorldUtilityWindowViewModel WorldUtilityWindowViewModel => _worldUtilityWindowViewModel;
        
        private WorldService _worldService;
        public WorldService WorldService => _worldService ??= _serviceLocator.Get<WorldService>();

        public string PlayerID => _playerProfile.ID;
        
        private DataService _dataService;
        public DataService DataService => _dataService ??= _serviceLocator.Get<DataService>();
        
        private UserInterfaceService _userInterfaceService;
        public UserInterfaceService UserInterfaceService => _userInterfaceService ??= _serviceLocator.Get<UserInterfaceService>();
    }
}