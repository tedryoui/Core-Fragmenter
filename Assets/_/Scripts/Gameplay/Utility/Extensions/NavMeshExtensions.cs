using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace _.Scripts.Gameplay.Utility.Extensions
{
    public static class NavMeshExtensions
    {
        public static bool GetNearestAccessiblePoint(this NavMeshAgent agent, float3 sourcePosition, float maxDistance,
            out                                           float3       validPosition)
        {
            validPosition = Vector3.zero;

            if (agent == null || maxDistance <= 0f)
                return false;

            int areaMask = agent.areaMask;

            if (NavMesh.SamplePosition(sourcePosition, out NavMeshHit hit, maxDistance, areaMask))
            {
                validPosition = hit.position;
                return true;
            }

            return false;
        }
    }
}