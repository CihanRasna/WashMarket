using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using GameplayScripts;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

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
        public List<Paydesk> payDesks = new List<Paydesk>();

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
            
            //ES3AutoSaveMgr.Current.Load();
            
            CheckForActiveMachineTypes();
            CalculateCornerPoints();
        }

        private void OnApplicationQuit()
        {
            ES3AutoSaveMgr.Current.Save();
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
                else if (currentMachine.GetType() == typeof(DryerMachine) && !dryerMachines.Contains(currentMachine as DryerMachine))
                {
                    dryerMachines.Add(currentMachine as DryerMachine);
                }
                else if (currentMachine.GetType() == typeof(IronMachine) && !ironMachines.Contains(currentMachine as IronMachine))
                {
                    ironMachines.Add(currentMachine as IronMachine);
                }
                else
                {
                    payDesks.Add(currentMachine as Paydesk);
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
                //Debug.Log("PAYDESK AQ", machine.gameObject);
                machine.occupied = false;
            }
            else
            {
                machine.occupied = true;
            }

            return machine;
        }

        private List<Vector3> _verticesList = new List<Vector3>();
        private readonly List<Vector3> _corners = new List<Vector3>();
        private readonly List<Vector3> _edgeVectors = new List<Vector3>();
        
        private void CalculateEdgeVectors(int vectorCorner)
        {
            _edgeVectors.Clear();

            _edgeVectors.Add(_corners[3] - _corners[vectorCorner]);
            _edgeVectors.Add(_corners[1] - _corners[vectorCorner]);
        }

        private void CalculateCornerPoints()
        {
            _verticesList = new List<Vector3>(waitingAreaDemo.mesh.vertices);
            _corners.Clear();
            
            _corners.Add(transform.TransformPoint(_verticesList[0])); //corner points are added to show  on the editor
            _corners.Add(transform.TransformPoint(_verticesList[10]));
            _corners.Add(transform.TransformPoint(_verticesList[110]));
            _corners.Add(transform.TransformPoint(_verticesList[120]));
        }

        public Vector3 CalculateRandomPoint()
        {
            int randomCornerIdx = Random.Range(0, 2) == 0 ? 0 : 2; 
            CalculateEdgeVectors(randomCornerIdx);

            var u = Random.Range(0.0f, 1.0f); 
            var v = Random.Range(0.0f, 1.0f);

            if (v + u > 1)
            {
                v = 1 - v;
                u = 1 - u;
            }

            return _corners[randomCornerIdx] + u * _edgeVectors[0] + v * _edgeVectors[1];
        }
    }
}