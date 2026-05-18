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
            return this;
        }

#endregion

#region Fields & Properties

        private string _identity;

        private float3     _position;
        public float3 Position => _position;

        private quaternion _rotation;
        public quaternion Rotation => _rotation;

        private string _targetIdentity;
        public string TargetIdentity
        {
            get => _targetIdentity;
            set => _targetIdentity = value;
        }

#endregion

        public DroneData(string identity)
        {
            _identity = identity;
        }
    }

    public class DroneDataPreset : IDataPreset
    {
        
    }
}