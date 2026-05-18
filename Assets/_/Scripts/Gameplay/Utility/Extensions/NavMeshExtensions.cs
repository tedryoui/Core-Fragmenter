using System;
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

        public static NavMeshAgent SetSoftDestination(
            this NavMeshAgent agent, 
            float3 sourcePosition,
            float? armLength = null,
            Func<float3, bool> validator = null)
        {
            int iterations = 0;

            while (iterations < 100)
            {
                iterations++;
                
                var randomDirection = math
                    .normalize(
                        new float3(
                            UnityEngine.Random.Range(-1f, 1f), 
                            0.0f, 
                            UnityEngine.Random.Range(-1f, 1f)
                        )
                    );
                var offset      = armLength ?? agent.stoppingDistance;
                var expecterPosition = sourcePosition + randomDirection * offset;
                
                var hasClosestPoint = agent
                    .GetNearestAccessiblePoint(
                        expecterPosition, 
                        float.MaxValue, 
                        out var validPosition
                    );

                if (hasClosestPoint)
                {
                    if (validator != null)
                    {
                        var result = validator.Invoke(validPosition);
                        
                        if (result)
                        {
                            agent.SetDestination(validPosition);
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        agent.SetDestination(validPosition);
                        break;
                    }
                }
                else
                {
                    throw new Exception("NavMeshAgent could not find suited position.");
                }
            }
            
            return agent;
        }

        public static NavMeshAgent Sleep(this NavMeshAgent agent)
        {
            agent.isStopped      = true;
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.updateUpAxis   = false;
            return agent;
        }

        public static NavMeshAgent WakeUp(this NavMeshAgent agent)
        {
            agent.isStopped      = false;
            agent.updatePosition = true;
            agent.updateRotation = true;
            agent.updateUpAxis   = true;
            return agent;   
        }
    }
}