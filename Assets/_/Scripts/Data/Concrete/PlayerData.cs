using System;
using Sirenix.OdinInspector;
using Unity.Mathematics;

namespace _.Scripts.Data.Concrete
{
    public class PlayerData : IData<PlayerDataPreset>
    {
        private string _identity;

        public string Identity => _identity;

        public bool   IsSigned => true;

#region Secondary Fields

        private float3     _position;
        private quaternion _rotation;
        private float      _currentSpeed;

#endregion

#region Primary Fields

        private float _speed;
        private float _angularSpeed;
        
#endregion

#region Secondary Properties

        public float3     Position
        {
            get => _position;
            set => _position = value;
        }

        public quaternion Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        public float CurrentSpeed
        {
            get => _currentSpeed;
            set => _currentSpeed = value;
        }

#endregion

#region Primary Properties

        public float Speed        => _speed;
        public float AngularSpeed => _angularSpeed;

#endregion

        public PlayerData(string identity)
        {
            _identity = identity;
            
            _position = float3.zero;
            _rotation = quaternion.identity;
            
            _currentSpeed = 0f;
        }

        public IData<PlayerDataPreset> Fill(PlayerDataPreset presetData)
        {
            _speed = presetData.Speed;
            _angularSpeed = presetData.AngularSpeed;

            return this;
        }
    }

    [Serializable, HideLabel, InlineProperty]
    public struct PlayerDataPreset : IDataPreset
    {
        public float Speed;
        public float AngularSpeed;
    }
}