using System;
using Sirenix.OdinInspector;
using Unity.Mathematics;

namespace _.Scripts.Data.Concrete
{
    public class DroneData : IData<DroneDataPreset>
    {
#region IData overrides

        public string Identity => _identity;
        public bool   IsSigned => false;

        public IData<DroneDataPreset> Fill(DroneDataPreset presetData)
        {
            _speed                     = presetData.Speed;
            _angularSpeed              = presetData.AngularSpeed;
            _dealDamageDelay           = presetData.DealDamageDelay;
            _nonChangePositionDelta    = presetData.AgentNonChangePositionDelta;
            _nonChangePositionDuration = presetData.AgentNonChangePositionDuration;
            _ammoMaximumAmount         = presetData.AmmoAmount;

            return this;
        }

#endregion

#region Fields & Properties

        private string _identity;

        private float3 _position;
        public  float3 Position
        {
            get => _position;
            set => _position = value;
        }

        private quaternion _rotation;
        public  quaternion Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        private string _targetIdentity;
        public string TargetIdentity
        {
            get => _targetIdentity;
            set => _targetIdentity = value;
        }

        private float _speed;
        public float Speed => _speed;

        private float _currentSpeed;
        public float CurrentSpeed
        {
            get => _currentSpeed;
            set => _currentSpeed = value;
        }
        
        private float _angularSpeed;
        public float AngularSpeed => _angularSpeed;
        
        private float _currentAngularSpeed;
        public float CurrentAngularSpeed
        {
            get => _currentAngularSpeed;
            set => _currentAngularSpeed = value;
        }
        
        private float _dealDamageDelay;
        public float DealDamageDelay => _dealDamageDelay;

        private float _dealDamageTime;
        public  float DealDamageTime
        {
            get => _dealDamageTime;
            set
            {
                _dealDamageTime = value switch
                {
                    var nextValue when nextValue < 0               => 0,
                    var nextValue when nextValue > DealDamageDelay => DealDamageDelay,
                    _                                              => value
                };
            }
        }
        
        private float _nonChangePositionDelta;
        public float NonChangePositionDelta => _nonChangePositionDelta;

        private float _nonChangePositionDuration;
        public float NonChangePositionDuration => _nonChangePositionDuration;

        private int _ammoMaximumAmount;
        public int AmmoMaximumAmount => _ammoMaximumAmount;
        
        private int _ammoCurrentAmount;
        public  int AmmoCurrentAmount
        {
            get => _ammoCurrentAmount;
            set
            {
                _ammoCurrentAmount = value switch
                {
                    var nextValue when nextValue < 0                 => 0,
                    var nextValue when nextValue > AmmoMaximumAmount => AmmoMaximumAmount,
                    _                                                => value
                };
            }
        }

#endregion

        public DroneData(string identity)
        {
            _identity = identity;
            
            _position = float3.zero;
            _rotation = quaternion.identity;

            _targetIdentity = "";

            _currentSpeed        = 0.0f;
            _currentAngularSpeed = 0.0f;

            _ammoCurrentAmount = 0;

            _dealDamageTime = 0.0f;
        }
    }

    [Serializable, HideLabel, InlineProperty]
    public struct DroneDataPreset : IDataPreset
    {
        public float Speed;
        public float AngularSpeed;

        [Unit(Units.Second)]
        public float DealDamageDelay;

        [PropertySpace] 
        public float AgentNonChangePositionDelta;
        [Unit(Units.Second)]
        public float AgentNonChangePositionDuration;

        public int AmmoAmount;
    }
}