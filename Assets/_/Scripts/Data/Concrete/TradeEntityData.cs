using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.Purchasing;

namespace _.Scripts.Data.Concrete
{
    public class TradeEntityData : IData<TradeEntityDataPreset>
    {
#region IData overrides

        public string Identity => _identity;
        public bool   IsSigned => true;
        
        public IData<TradeEntityDataPreset> Fill(TradeEntityDataPreset presetData)
        {
            _maxOrderQuantity = 1;
            
            return this;
        }
        
#endregion
        
#region Structures

        public class TradeOrder
        {
            private DateTime _identity = default;
            private DateTime _timestamp = default;
            
            private readonly List<TradeOrderLine> _lines = new();

            public string Identity => _identity.ToString(CultureInfo.InvariantCulture);
            
            public IReadOnlyList<TradeOrderLine> Lines => _lines;

            public bool  HasItems      => _lines.Count > 0;
            public float TotalSeconds  => (float)(_timestamp - _identity).TotalSeconds;
            public float SecondsRemain => (float)(_timestamp - DateTime.Now).TotalSeconds;

            public void SetIdentity(DateTime identity)
            {
                _identity = identity;
            }

            public void SetTimestamp(DateTime timestamp)
            {
                _timestamp = timestamp;
            }

            public void Clear() => _lines.Clear();

            public bool TryGetLine(string identity, out TradeOrderLine line)
            {
                line = default;

                if (string.IsNullOrEmpty(identity))
                    return false;

                var index = _lines.FindIndex(entry => entry.Identity == identity);
                if (index < 0)
                    return false;

                line = _lines[index];
                return true;
            }

            public void SetLine(string identity, int quantity)
            {
                if (string.IsNullOrEmpty(identity))
                    return;

                if (quantity <= 0)
                {
                    RemoveLine(identity);
                    return;
                }

                var index = _lines.FindIndex(entry => entry.Identity == identity);
                if (index < 0)
                    _lines.Add(new TradeOrderLine(identity, quantity));
                else
                    _lines[index] = new TradeOrderLine(identity, quantity);
            }

            public void AddLine(string identity, int quantity = 1) =>
                SetLine(identity, TryGetLine(identity, out var line) ? line.Quantity + quantity : quantity);

            public bool RemoveLine(string identity)
            {
                if (string.IsNullOrEmpty(identity))
                    return false;

                var index = _lines.FindIndex(entry => entry.Identity == identity);
                if (index < 0)
                    return false;

                _lines.RemoveAt(index);
                return true;
            }

            public bool IsCompleted()
            {
                return DateTime.Compare(DateTime.Now,  _timestamp) >= 0;
            }
        }

        public readonly struct TradeOrderLine
        {
            public TradeOrderLine(string identity, int quantity)
            {
                Identity = identity;
                Quantity = quantity;
            }

            public string Identity { get; }
            public int    Quantity { get; }
        }
        
#endregion
        
        public TradeEntityData(string identity)
        {
            _identity = identity;
            _orders   = new List<TradeOrder>();
        }

#region Fields & Properties

        private string _identity;
        
        private int _maxOrderQuantity;
        public int MaxOrderQuantity => _maxOrderQuantity;

        private List<TradeOrder>                _orders;
        public  IReadOnlyCollection<TradeOrder> Orders => _orders;

#endregion

        public void AddOrder(TradeOrder order, DateTime whenCompletes)
        {
            if (Orders.Count >= _maxOrderQuantity)
                return;
            
            order.SetIdentity(DateTime.Now);
            order.SetTimestamp(whenCompletes);
            
            _orders.Add(order);
        }

        public void RemoveOrder(string identity)
        {
            if (string.IsNullOrEmpty(identity))
                return;
            
            _orders.RemoveAll(x => x.Identity.Equals(identity));
        }

        public bool HasOrder(string identity)
        {
            return Orders.Any(x => x.Identity.Equals(_identity));
        }

        public TradeOrder GetOrder(string identity)
        {
            if (string.IsNullOrEmpty(identity))
                throw new ArgumentNullException(nameof(identity));
            
            return Orders.First(x => x.Identity.Equals(_identity));
        }
    }

    [Serializable, InlineProperty, HideLabel]
    public struct TradeEntityDataPreset : IDataPreset
    {
    }
}
