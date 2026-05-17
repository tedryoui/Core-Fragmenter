using Unity.Mathematics;

namespace _.Scripts.Gameplay.Utility.Extensions
{
    public static class Float3Extensions
    {
        public static quaternion ToQuaternionFromEulerDegrees(this float3 value)
        {
            return quaternion.EulerXYZ(
                x: math.radians(value.x),
                y: math.radians(value.y),
                z: math.radians(value.z));
        }
    }
}