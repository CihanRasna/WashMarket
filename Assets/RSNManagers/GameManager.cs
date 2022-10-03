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

        [SerializeField] private List<Actor> possibleCustomers;
        public List<Machine> allMachines;
        public Transform leavePos;
        public ClothPicker clothPicker;
        [SerializeField] private Vector3 lastKnownPosOfPlayer;
        

        protected override void Awake()
        {
            base.Awake();
            //Application.targetFrameRate = 60;
            LoadGame();
        }

        protected override void Start()
        {
            base.Start();
            //StartGame();
        }

        private void LoadGame()
        {
            if (currentState == GameStates.Loaded) return;
            currentState = GameStates.Loaded;

            if (currentPlayer) return;
            currentPlayer = Instantiate(playerPrefab) as Player;
            currentPlayer.transform.position = PersistManager.Instance.PlayersLastPos;

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

        private IEnumerator TestMe()
        {
            var customer = Instantiate(possibleCustomers[0], leavePos.position,Quaternion.identity);
            yield return new WaitForSeconds(10f);
            StartCoroutine(TestMe());
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
                if (currentMachine.GetType() == desiredType && !currentMachine.occupied)
                {
                    var dist = (lookerTransform.position - machineTransform.position).sqrMagnitude;
                    if (dist <= closestDist)
                    {
                        closestDist = dist;
                        machine = currentMachine;
                    }
                }
            }

            if (machine == null) return null;
            if (desiredType == typeof(Paydesk))
            {
                Debug.Log("PAYDESK AQ",machine.gameObject);
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