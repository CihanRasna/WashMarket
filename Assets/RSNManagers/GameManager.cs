using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using GameplayScripts;
using JetBrains.Annotations;
using UnityEngine;

namespace RSNManagers
{
    public class GameManager : Singleton<GameManager>
    {
        private enum GameStates
        {
            Loading,
            Loaded,
            Started,
            Placement,
            Paused,
            Failed,
        }

        [SerializeField] private GameStates currentState = GameStates.Loading;

        [SerializeField] private Actor playerPrefab;
        public Player currentPlayer;
        [SerializeField] private CinemachineVirtualCamera playerCamera;
        [SerializeField] private DayNightCycle shiftTimer;

        [SerializeField] private List<Actor> possibleCustomers;
        public List<Machine> allMachines;
        public Transform leavePos;
        public MeshFilter waitingAreaDemo;

        public ClothPicker clothPicker;

        public bool shopHasWasher = false;
        public bool shopHasDryer = false;
        public bool shopHasIronTable = false;
        public event Action ShiftEndedAction;

        public List<WashingMachine> washingMachines = new List<WashingMachine>();
        public List<DryerMachine> dryerMachines = new List<DryerMachine>();
        public List<IronMachine> ironMachines = new List<IronMachine>();

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
            LoadGame();
        }

        protected override void Start()
        {
            base.Start();
            //StartGame();

            shiftTimer.ShiftStartedAction += ShiftStarted;
            shiftTimer.ShiftEndedAction += ShiftEnded;
            
            CheckForActiveMachineTypes();
        }

        private void MachineListSeparator()
        {
            var currentMachines = allMachines;
            
            for (var i = 0; i < currentMachines.Count; i++)
            {
                var currentMachine = currentMachines[i];
                if (currentMachine.GetType() == typeof(WashingMachine) && !washingMachines.Contains(currentMachine as WashingMachine))
                {
                    washingMachines.Add(currentMachine as WashingMachine);
                }
                if (currentMachine.GetType() == typeof(DryerMachine) && !dryerMachines.Contains(currentMachine as DryerMachine))
                {
                    dryerMachines.Add(currentMachine as DryerMachine);
                }
                if (currentMachine.GetType() == typeof(IronMachine) && !ironMachines.Contains(currentMachine as IronMachine))
                {
                    ironMachines.Add(currentMachine as IronMachine);
                }
            }
        }

        public void CheckForActiveMachineTypes() //TODO : NEED FIX FOR PERFORMANCE ISSUE
        {
            MachineListSeparator();

            shopHasWasher = washingMachines.Count != 0;
            shopHasDryer = dryerMachines.Count != 0;
            shopHasIronTable = ironMachines.Count != 0;
            
            clothPicker.usableClothTypes = new List<CustomerItem>(clothPicker.allClothTypes);

            for (var i = 0; i < clothPicker.allClothTypes.Count; i++)
            {
                var currentType = clothPicker.allClothTypes[i];

                if (!shopHasWasher && currentType.needWash)
                {
                    clothPicker.usableClothTypes.Remove(currentType);
                }

                if (!shopHasDryer && currentType.needDry)
                {
                    clothPicker.usableClothTypes.Remove(currentType);
                }

                if (!shopHasIronTable && currentType.needIron)
                {
                    clothPicker.usableClothTypes.Remove(currentType);
                }
            }
        }

        private void ShiftStarted(float shiftTime)
        {
            StartCoroutine(SpawnCustomers(shiftTime));
        }

        private void ShiftEnded()
        {
            ShiftEndedAction.Invoke();
        }

        private void LoadGame()
        {
            if (currentState == GameStates.Loaded) return;
            currentState = GameStates.Loaded;

            if (currentPlayer) return; //TODO : LOGIC FIX
            currentPlayer ??= Instantiate(playerPrefab) as Player;
            //currentPlayer.transform.position = PersistManager.Instance.PlayersLastPos;

            if (!currentPlayer) return;
            var currentPlayerTransform = currentPlayer.transform;
            playerCamera.Follow = currentPlayerTransform;
            playerCamera.LookAt = currentPlayerTransform;
        }

        public void StartGame()
        {
            if (currentState is GameStates.Loaded or GameStates.Paused)
            {
                Debug.Log("STARTED");
                currentState = GameStates.Started;
                //StartCoroutine(TestMe());
            }
        }

        private IEnumerator SpawnCustomers(float totalShiftTime)
        {
            var machineCount = allMachines.Count;
            var machineDelayPerCustomer = 30f;
            var spawnDelay = machineDelayPerCustomer / machineCount;
            var totalPossibleCustomerCount = (int)(totalShiftTime / spawnDelay);
            var waitForSeconds = new WaitForSeconds(spawnDelay);

            for (var i = 0; i < totalPossibleCustomerCount; i++)
            {
                var customer = Instantiate(possibleCustomers[0], leavePos.position, Quaternion.identity) as Customer;
                ShiftEndedAction += customer.ShopClosed;
                yield return waitForSeconds;
            }
        }

        private void PauseGame()
        {
            if (currentState is GameStates.Started or GameStates.Loaded)
            {
                currentState = GameStates.Paused;
            }
        }

        private void ResumeGame()
        {
            if (currentState == GameStates.Paused)
            {
                currentState = GameStates.Started;
            }
        }

        private void FailGame()
        {
            if (currentState is (GameStates.Loaded or GameStates.Started) and not GameStates.Failed)
            {
                currentState = GameStates.Failed;
                UIManager.Instance.OpenFailedPanel();
            }
        }

        [CanBeNull]
        public Machine FindClosestMachine(Customer.WorkType workType, Transform lookerTransform)
        {
            Machine machine = null;
            var desiredType = workType switch
            {
                Customer.WorkType.Wash => typeof(WashingMachine),
                Customer.WorkType.Dry => typeof(DryerMachine),
                Customer.WorkType.Iron => typeof(IronMachine),
                Customer.WorkType.Pay => typeof(Paydesk),
                _ => throw new ArgumentOutOfRangeException(nameof(workType), workType, null)
            };
            var closestDist = float.PositiveInfinity;

            for (var i = 0; i < allMachines.Count; i++)
            {
                var currentMachine = allMachines[i];
                var machineTransform = currentMachine.transform;
                if (currentMachine.GetType() == desiredType)
                {
                    if (!currentMachine.occupied)
                    {
                        var dist = (lookerTransform.position - machineTransform.position).sqrMagnitude;
                        if (dist <= closestDist)
                        {
                            closestDist = dist;
                            machine = currentMachine;
                        }
                    }
                }
            }

            if (machine == null) return null;
            if (desiredType == typeof(Paydesk))
            {
                Debug.Log("PAYDESK AQ", machine.gameObject);
                machine.occupied = false;
            }
            else
            {
                machine.occupied = true;
            }

            return machine;
        }
    }
}