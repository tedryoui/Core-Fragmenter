using System;

namespace _.Scripts.Services
{
    public interface IService : IDisposable
    {
        public void Initialize();
    }
}