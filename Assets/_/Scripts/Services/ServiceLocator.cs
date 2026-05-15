using System;
using System.Collections.Generic;
using System.Linq;
using _.Scripts.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ServiceLocator : IInitializable
{
    private IObjectResolver _objectResolver;
    
    private Dictionary<string, IService> _services = new Dictionary<string, IService>();
    
    public IReadOnlyDictionary<string, IService> Services => _services;

    [Inject]
    private void Configure(IObjectResolver resolver)
    {
        _objectResolver = resolver;
    }
    
    public void Initialize()
    {
        _services = new Dictionary<string, IService>();
        
        Debug.Log($"<color=green>{nameof(ServiceLocator)} created!</color>");
    }

    public override string ToString()
    {
        return $"SERVICE LOCATOR STATE: {_services.Count}";
    }

    public void Add(IService service, string identity)
    {
        if (_services.TryAdd(identity, service))
        {
            _objectResolver.Inject(service);
            service.Initialize();
            
            Debug.Log($"<color=yellow>{identity} has been added!</color>");
        }
        else
        {
            Debug.Log($"<color=red>{identity} hasn't been added!</color>");
        }
    }

    public void Remove(string identity)
    {
        var service = _services.GetValueOrDefault(identity);
        
        if (service is not null)
        {
            _services.Remove(identity);
            service.Dispose();
            
            Debug.Log($"<color=yellow>{identity} has been removed!</color>");
        }
        else
        {
            Debug.Log($"<color=red>{identity} hasn't been removed!</color>");
        }
    }

    public T Get<T>(string identity = "")
        where T : class, IService
    {
        if (string.IsNullOrEmpty(identity))
        {
            var result = _services.FirstOrDefault(x => x.Value.GetType() == typeof(T));

            if (result.Value is not null)
                return result.Value as T;
            else 
                throw new NullReferenceException($"Service {typeof(T)} not found!");
        }
        else
        {
            if (_services.TryGetValue(identity, out var result))
            {
                if (result is T tResult)
                    return tResult;
                else
                    throw new NullReferenceException($"Service {identity} and type {typeof(T)} not found!");
            }
            else
            {
                throw new NullReferenceException($"Service {identity} not found!");
            }
        }
    }

    public IService Get(Type T, string identity = "")
    {
        if (string.IsNullOrEmpty(identity))
        {
            var result = _services.FirstOrDefault(x => x.Value.GetType() == T);

            if (result.Value is not null)
                return result.Value;
            else 
                throw new NullReferenceException($"Service {nameof(T)} not found!");
        }
        else
        {
            if (_services.TryGetValue(identity, out var result))
            {
                return result;
            }
            else
            {
                throw new NullReferenceException($"Service {identity} not found!");
            }
        }
    }
}
