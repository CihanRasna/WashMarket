using System;
using ES3Internal;
using RSNManagers;
using Sirenix.OdinInspector;
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
        [SerializeField] private protected float durability;
        [SerializeField] protected float capacity;
        [SerializeField] protected float consumption;
        [SerializeField] protected int usingPrice;
        [SerializeField] protected Machine nextLevelMachine;
        [SerializeField] protected Animator animator;

        public int UsingPrice => usingPrice;
        private event Action WorkDoneAction;

        protected Customer _currentCustomer;
        protected float remainDurability;
        protected CustomerItem _customerItems;
        protected float _workedTime = 0;
        protected bool _needsRepair = false;

        public bool occupied;
        [field: SerializeField] public bool Filled { get; protected set; }

        [Button]
        private void Sell()
        {
            ES3AutoSaveMgr.RemoveAutoSave(GetComponent<ES3AutoSave>());
            var asd = ES3ReferenceMgrBase.Current as ES3ReferenceMgr;
            asd.Optimize();
            asd.RefreshDependencies();
            Destroy(gameObject);
        }

        protected virtual void Start()
        {
            if (remainDurability == 0 && !_needsRepair)
            {
                remainDurability = durability;
            }
            _workedTime = 0f;
            occupied = false;
            GameManager.Instance.allMachines.Add(this);
        }

        protected virtual void Working()
        {
            var deltaTime = Time.deltaTime;
            if (remainDurability <= 0)
            {
                if (!_needsRepair)
                {
                    _needsRepair = true;
                    NeedRepair();
                }
                return;
            }

            _workedTime += deltaTime;
            remainDurability -= deltaTime;
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

        public void NeedRepair()
        {
            return;
        }

        #region AnimatorOverrideMethods

        public abstract void StartInteraction();
        public abstract void CurrentlyWorking();
        public abstract void FinishInteraction();


        #endregion
    }
}