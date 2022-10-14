using System;
using System.Collections;
using DG.Tweening;
using GameplayScripts.Characters;
using GameplayScripts.Cloth;
using RSNManagers;
using UnityEngine;
using UnityEngine.AI;

namespace GameplayScripts.Machines
{
    [Serializable, SelectionBase]
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
        [SerializeField] private float maxDurability;
        [SerializeField] protected float durability;
        [SerializeField] protected float capacity;
        [SerializeField] protected float consumption;
        [SerializeField] protected int usingPrice;
        [SerializeField] protected Animator animator;
        [SerializeField] protected int buyPrice = 100;
        [SerializeField] private int sellPrice;
        [SerializeField] private int totalGain;

        [SerializeField] protected LayerMask unplaceableLayers;
        [SerializeField] protected Transform machineMesh;
        [SerializeField] protected Renderer machineButton;

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
        public bool Repairing { get; private set; }

        protected GameManager Manager;

        public bool occupied;
        private static readonly int MachineBroken = Animator.StringToHash("MachineBroken");
        private static readonly int Repaired = Animator.StringToHash("MachineRepaired");

        [field: SerializeField] public bool Filled { get; protected set; }

        public (string machineName, Level currentLevel, float singleWorkTime, float durability, int usingPrice, int
            buyPrice, float capacity, float consumption, int sellPrice, int totalGain, int repairPrice) RefValuesForUI()
        {
            RepairPricing(out var repairPrice, out var ratio,out var sellPricing);
            return (machineName, currentLevel, singleWorkTime, durability, usingPrice, buyPrice, capacity, consumption,
                sellPricing, totalGain, repairPrice);
        }

        public virtual void Sell(out int price)
        {
            price = SellPricing();
            Manager.allMachines.Remove(this);
        }

        protected virtual void Awake()
        {
            Manager = GameManager.Instance;
            Manager.allMachines.Add(this);
        }

        protected virtual void Start()
        {
            var mTransform = transform;
            var euler = mTransform.localEulerAngles;
            mTransform.localEulerAngles = new Vector3(0, euler.y, 0);

            navMeshObstacle ??= GetComponent<NavMeshObstacle>();

            ButtonColorChanger(Color.green);

            if (remainDurability <= 0f)
            {
                if (_needsRepair)
                {
                    RepairBehaviourOverride();
                }
                else
                {
                    remainDurability = durability;
                }
            }
            else
            {
                occupied = false;
            }

            navMeshObstacle.enabled = obstacleEnabled;
            _workedTime = 0f;
        }

        private void ButtonColorChanger(Color color)
        {
            if (machineButton)
            {
                machineButton.material.color = Color.white;
                DOTween.Kill(333);
                machineButton.material.DOColor(color, 1f).SetLoops(-1, LoopType.Yoyo).SetId(333);
            }
        }

        protected virtual void Working()
        {
            if (Repairing) return;
            
            var deltaTime = Time.deltaTime;
            if (remainDurability <= 0)
            {
                if (!_needsRepair)
                {
                    _needsRepair = true;
                    RepairBehaviourOverride();
                }

                return;
            }

            _workedTime += deltaTime;
            remainDurability -= deltaTime * consumption;
            if (_workedTime >= singleWorkTime)
            {
                DoneWork();
            }
        }

        public void MachineIsOccupied(Customer customer)
        {
            _currentCustomer = customer;
            WorkDoneAction += _currentCustomer.MachineFinished;
        }

        public void StartWork(Customer customer)
        {
            ButtonColorChanger(Color.red);

            if (!Filled)
            {
                Filled = true;
                CurrentlyWorking();
                //START EVENTS LIKE A ANIMS
            }
        }

        public void DoneWork()
        {
            ButtonColorChanger(Color.green);
            WorkDoneAction.Invoke();
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

        public void RepairPricing(out int repairPrice, out float ratio, out int sellPricing)
        {
            ratio = Mathf.Clamp01(remainDurability / durability);
            var totalCorruption = Mathf.Clamp01(durability / maxDurability);
            var halfPrice = buyPrice * 0.5f * totalCorruption;
            repairPrice = Mathf.FloorToInt(halfPrice * (1 - ratio));
            sellPricing = SellPricing();
        }

        private int SellPricing()
        {
            var nonCorruptPrice = buyPrice;
            var currentCorruption = Mathf.Clamp01(durability / maxDurability);
            if (float.IsNaN(currentCorruption))
            {
                currentCorruption = 1f;
            }
            return Mathf.FloorToInt(nonCorruptPrice * currentCorruption);
        }

        public void MachineRepairing()
        {
            Repairing = true;
            RepairPricing(out var repairPrice, out var ratio,out var sellPricing);
            var corruptAmount = maxDurability * 0.1f;
            durability -= (1f - ratio) * corruptAmount;
            DOTween.To(() => remainDurability, x => remainDurability = x, durability, 2f).OnComplete(() =>
            {
                
                if (_needsRepair)
                    animator.SetTrigger(Repaired);

                Repairing = false;
                _needsRepair = false;
            });
        }

        public void INeedRepair()
        {
            ButtonColorChanger(Color.white);
            _needsRepair = true;

            CurrentlyWorking();
            animator.SetTrigger(MachineBroken);
        }

        protected virtual void RepairBehaviourOverride()
        {
            INeedRepair();
        }

        #region AnimatorOverrideMethods

        public abstract void StartInteraction();
        public abstract void CurrentlyWorking();
        public abstract void FinishInteraction();

        #endregion
    }
}