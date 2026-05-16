using System;
using System.Collections.Generic;
using System.Linq;
using _.Scripts.User_Interface;
using UnityEngine;
using VContainer;

namespace _.Scripts.Services
{
    public class UserInterfaceService : IService
    {
        private IObjectResolver                                    _objectResolver;
        
        private Dictionary<string, AbstractUserInterfaceViewModel> _userInterfaces;

        [Inject]
        private void Configure(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }
        
        public void Initialize()
        {
            _userInterfaces = new Dictionary<string, AbstractUserInterfaceViewModel>();
            
            Debug.Log($"<color=green>{nameof(UserInterfaceService)} initialized!</color>");
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        public void Add(string identity, AbstractUserInterfaceViewModel viewModel)
        {
            if (_userInterfaces.TryAdd(identity, viewModel))
            {
                Debug.Log($"{identity} added!");
            }
            else
            {
                Debug.Log($"{identity} is already registered!");
            }
        }

        public void Remove(string identity)
        {
            if (_userInterfaces.Remove(identity))
            {
                Debug.Log($"{identity} removed!");
            }
            else
            {
                Debug.Log($"{identity} is not registered!");
            }
        }

        public bool Has<T>(string identity = "") 
            where T : AbstractUserInterfaceViewModel
        {
            if (string.IsNullOrEmpty(identity))
            {
                var result = _userInterfaces.FirstOrDefault(x => x.Value.GetType() == typeof(T));

                if (result.Value is not null)
                    return true;
                else
                    return false;
            }
            else
            {
                if (_userInterfaces.TryGetValue(identity, out var result))
                {
                    if (result is T tResult)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
        }

        public T Get<T>(string identity = "")
            where T : AbstractUserInterfaceViewModel
        {
            if (string.IsNullOrEmpty(identity))
            {
                var result = _userInterfaces.FirstOrDefault(x => x.Value.GetType() == typeof(T));

                if (result.Value is not null)
                    return result.Value as T;
                else 
                    throw new NullReferenceException($"View model {typeof(T)} not found!");
            }
            else
            {
                if (_userInterfaces.TryGetValue(identity, out var result))
                {
                    if (result is T tResult)
                        return tResult;
                    else
                        throw new NullReferenceException($"View model {identity} and type {typeof(T)} not found!");
                }
                else
                {
                    throw new NullReferenceException($"View model {identity} not found!");
                }
            }
        }

        public AbstractUserInterfaceViewModel Get(Type T, string identity = "")
        {
            if (string.IsNullOrEmpty(identity))
            {
                var result = _userInterfaces.FirstOrDefault(x => x.Value.GetType() == T);

                if (result.Value is not null)
                    return result.Value;
                else 
                    throw new NullReferenceException($"View model {nameof(T)} not found!");
            }
            else
            {
                if (_userInterfaces.TryGetValue(identity, out var result))
                {
                    return result;
                }
                else
                {
                    throw new NullReferenceException($"View model {identity} not found!");
                }
            }
        }
    }
}