using System;
using System.Collections.Generic;
using System.Linq;
using _.Scripts.Data;
using UnityEngine;

namespace _.Scripts.Services
{
    public class DataService : IService
    {
        private List<Type> DataToInitialize => new List<Type>()
        {
        };

        private List<IData>               _dataCache;
        private Dictionary<string, IData> _signedDataCache;
        
        public void Initialize()
        {
            _dataCache = new List<IData>();
            _signedDataCache = new Dictionary<string, IData>();
            
            Debug.Log($"<color=green>{nameof(DataService)} initialized!</color>");
        }

        public void Dispose()
        {
            Debug.Log($"<color=red>{nameof(DataService)} disposed!</color>");
        }

        public void Add(IData data)
        {
            var has = _dataCache.Any(x => x.Identity == data.Identity);
            
            if (has)
                throw new Exception($"Data {data.Identity} already exists");
            
            _dataCache.Add(data);
            
            if (data.IsSigned)
                _signedDataCache.Add(data.Identity, data);
            
            Debug.Log($"New data {data.Identity} added! {typeof(IData)})");
        }

        public void Remove(string identity)
        {
            var has = _dataCache.Any(x => x.Identity == identity);
            
            if (!has)
                throw new Exception($"Data {identity} does not exist");
            
            var data = _dataCache.First(x => x.Identity == identity);
            
            _dataCache.Remove(data);
            if (data.IsSigned)
                _signedDataCache.Remove(data.Identity);
            
            Debug.Log($"Data {identity} removed!");
        }

        public T Get<T>(string identity) 
            where T : IData
        {
            if (_signedDataCache.TryGetValue(identity, out var data) && data is T tData)
                return tData;
            
            var result = _dataCache.FirstOrDefault(x => x.Identity == identity && x is T);
            
            if (result != null)
                return (T)result;
            
            throw new Exception($"Data {identity} does not exist");
        }

        public bool Has<T>(string identity)
        {
            if (_signedDataCache.TryGetValue(identity, out var data) && data is T)
                return true;
            
            var result = _dataCache.FirstOrDefault(x => x.Identity == identity && x is T);
            
            if (result != null)
                return true;

            return false;
        }
    }
}