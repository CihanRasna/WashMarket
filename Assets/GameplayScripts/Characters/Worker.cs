using RSNManagers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameplayScripts.Characters
{
    public abstract class Worker : Actor
    {
        protected static readonly int Blend = Animator.StringToHash("Blend");
        [SerializeField] private bool isPlayer;

        protected GameManager GameManager;
        
        protected enum WorkerType
        {
            Player,
            Clerk,
            Mechanic,
            Cleaner
        }

        protected enum State
        {
            Idle,
            BreakTime,
            GoingForWork,
            Working,
            ShiftEnded
        }

        [SerializeField,HideIf("isPlayer")] protected WorkerType workerType;
        [SerializeField,HideIf("isPlayer")] protected State state;

        protected virtual void Start()
        {
            GameManager = GameManager.Instance;
            GameManager.allWorkers.Add(this);
        }
    }
}
