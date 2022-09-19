using System;
using RSNManagers;
using UnityEngine;

namespace GameplayScripts
{
    [Serializable]
    public abstract class Machine : MonoBehaviour, IWorkable, IRepairable
    {
        public enum Level
        {
            Level1 = 1,
            Level2 = 2,
            Level3 = 3
        }

        public Level currentLevel;

        [SerializeField] protected float singleWorkTime;
        [SerializeField] protected float durability;
        [SerializeField] protected float capacity;
        [SerializeField] protected float consumption;
        [SerializeField] protected int usingPrice;
        [SerializeField] protected Machine nextLevelMachine;
        [SerializeField] protected Animator animator;

        public int UsingPrice => usingPrice;

        private event Action WorkDoneAction;

        protected Customer _currentCustomer;
        protected CustomerItem _customerItems;
        protected float _workedTime = 0;
        protected int _durabilityWashCount;

        public bool occupied;
        [field: SerializeField] public bool Filled { get; protected set; }

        protected virtual void Start()
        {
            _workedTime = 0f;
            occupied = false;
            _durabilityWashCount = (int)(durability / singleWorkTime);
            GameManager.Instance.allMachines.Add(this);
        }

        protected virtual void Working()
        {
            var deltaTime = Time.deltaTime;
            if (durability <= 0 || _durabilityWashCount == 0)
            {
                Repair();
                return;
            }

            _workedTime += deltaTime;
            durability -= deltaTime;
            if (_workedTime >= singleWorkTime)
            {
                DoneWork();
            }
        }

        public void MachineIsOccupied(Customer customer)
        {
            _currentCustomer = customer;
        }

        public void StartWork(Customer customer)
        {
            WorkDoneAction += _currentCustomer.MachineFinished;

            if (!Filled)
            {
                Filled = true;
                CurrentlyWorking();
                //START EVENTS LIKE A ANIMS
            }
        }

        public void DoneWork()
        {
            WorkDoneAction?.Invoke();
            _durabilityWashCount -= 1;
            _workedTime = 0f;
            Filled = false;
            CurrentlyWorking();
        }

        public void Empty()
        {
            WorkDoneAction -= _currentCustomer.MachineFinished;
            _currentCustomer = null;
            occupied = false;
        }

        public void Repair()
        {
        }

        #region AnimatorOverrideMethods

        public abstract void StartInteraction();
        public abstract void CurrentlyWorking();
        public abstract void FinishInteraction();


        #endregion
    }
}