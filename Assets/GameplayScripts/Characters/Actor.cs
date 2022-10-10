using UnityEngine;
using UnityEngine.AI;

namespace GameplayScripts.Characters
{
    public abstract class Actor : MonoBehaviour
    {
        [SerializeField] protected float speed = 3f;
        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] private protected Animator animator;
    }
}