using System;
using RSNManagers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace GameplayScripts
{
    [Serializable,SelectionBase]
    public abstract class Machine : MonoBehaviour, IWorkable, IRepairable
    {
        public enum Level
        {
            Level1 = 1,
            Level2 = 2,
            Level3 = 3
        }

        public Level currentLevel;

        [SerializeField] private string machineName;
        [SerializeField] protected float singleWorkTime;
        [SerializeField] private protected float durability;
        [SerializeField] protected float capacity;
        [SerializeField] protected float consumption;
        [SerializeField] protected int usingPrice;
        [SerializeField] protected Animator animator;
        [SerializeField] protected int buyPrice = 100;
        [SerializeField] private int sellPrice;
        [SerializeField] private int totalGain;

        [SerializeField] protected LayerMask unplaceableLayers;
        [SerializeField] protected Transform machineMesh;
        public NavMeshObstacle navMeshObstacle;
        public bool obstacleEnabled = false;

        public LayerMask UnplaceableLayers => unplaceableLayers;
        public int UsingPrice => usingPrice;
        public int BuyPrice => buyPrice;
        public Transform MeshObject => machineMesh;
        public float RemainDurability => remainDurability;
        private event Action WorkDoneAction;

        protected Customer _currentCustomer;
        protected float remainDurability;
        protected CustomerItem _customerItems;
        protected float _workedTime = 0;
        protected bool _needsRepair = false;
        
        protected GameManager Manager;

        public bool occupied;
        [field: SerializeField] public bool Filled { get; protected set; }

        public (string machineName, Level currentLevel, float singleWorkTime, float durability, int usingPrice, int
            buyPrice, float capacity, float consumption, int sellPrice, int totalGain) RefValuesForUI()
        {
            return (machineName, currentLevel, singleWorkTime, durability, usingPrice, buyPrice, capacity, consumption,
                sellPrice, totalGain);
        }
        public virtual void Sell(out int price)
        {
            price = sellPrice;
        }
        protected virtual void Awake()
        {
            Manager = GameManager.Instance;
            Manager.allMachines.Add(this);
        }

        protected virtual void Start()
        {
            navMeshObstacle ??= GetComponent<NavMeshObstacle>();

            if (remainDurability == 0 && !_needsRepair)
            {
                remainDurability = durability;
            }

            navMeshObstacle.enabled = obstacleEnabled;
            _workedTime = 0f;
            occupied = false;
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
            totalGain += usingPrice;
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