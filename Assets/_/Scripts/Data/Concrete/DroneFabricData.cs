using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace _.Scripts.Data.Concrete
{
    public class DroneFabricData : IData<DroneFabricDataPreset>
    {
        public  string Identity => _identity;
        public  bool   IsSigned => true;

        public IData<DroneFabricDataPreset> Fill(DroneFabricDataPreset presetData)
        {
            _maxActiveOrders = 1;
            return this;
        }

        [Serializable]
        public struct Order
        {
            public string   Identity;
            public DateTime Start;
            public DateTime End;

            public int TotalSeconds => (int)(End - Start).TotalSeconds;
            public int RemainingSeconds => (int)(End - DateTime.Now).TotalSeconds;
            
            public bool IsCompleted()
            {
                return DateTime.Compare(DateTime.Now, End) > 0;
            }
        }
        
        private string _identity;

        private int _maxActiveOrders;
        public  int MaxActiveOrders => _maxActiveOrders;
        
        private List<Order> _activeOrders;
        public IReadOnlyCollection<Order> ActiveOrders => _activeOrders;

        public DroneFabricData(string identity)
        {
            _identity        = identity;
            _maxActiveOrders = 0;
            _activeOrders    = new List<Order>();
        }

        public void AddOrder(Order order)
        {
            if (ActiveOrders.Count >= _maxActiveOrders)
                throw new StackOverflowException("Max active orders reached!"); 
            
            _activeOrders.Add(order);
        }

        public Order GetOrder(string identity)
        {
            if (HasOrder(identity))
                return _activeOrders.FirstOrDefault(x => x.Identity.Equals(identity));

            throw new KeyNotFoundException($"Order {identity} not found!");
        }

        public bool HasOrder(string identity)
        {
            return _activeOrders.Any(x => x.Identity.Equals(identity));
        }

        public void RemoveOrder(string identity)
        {
            if (HasOrder(identity))
                _activeOrders.RemoveAll(x => x.Identity.Equals(identity));
            
            throw new KeyNotFoundException($"Order {identity} not found!");
        }
    }

    [Serializable, InlineProperty, HideLabel]
    public struct DroneFabricDataPreset : IDataPreset
    {
    }
}