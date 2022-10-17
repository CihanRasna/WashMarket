using UnityEngine;
using UnityEngine.AI;

namespace GameplayScripts.Characters
{
    public abstract class Actor : MonoBehaviour
    {
        [SerializeField] protected float speed = 3f;
        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] protected Animator animator;
        
        protected bool AgentIsArrived()
        {
            return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance &&
                   (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }
    }
}