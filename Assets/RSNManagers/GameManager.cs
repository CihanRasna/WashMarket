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
            Paused,
            Failed,
        }

        [SerializeField] private GameStates currentState = GameStates.Loading;

        [SerializeField] private Actor playerPrefab;
        public Player currentPlayer;
        [SerializeField] private CinemachineVirtualCamera playerCamera;

        [SerializeField] private List<Actor> possibleCustomers;
        public List<Machine> allMachines;
        public Paydesk paydesk;
        public Transform leavePos;

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
            LoadGame();
        }

        protected override void Start()
        {
            base.Start();
            StartGame();
        }

        private void LoadGame()
        {
            if (currentState == GameStates.Loaded) return;
            currentState = GameStates.Loaded;

            if (currentPlayer) return;
            currentPlayer = Instantiate(playerPrefab, transform.position, Quaternion.identity) as Player;

            if (!currentPlayer) return;
            var currentPlayerTransform = currentPlayer.transform;
            playerCamera.Follow = currentPlayerTransform;
            playerCamera.LookAt = currentPlayerTransform;
        }

        private void StartGame()
        {
            if (currentState is GameStates.Loaded or GameStates.Paused)
            {
                currentState = GameStates.Started;
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
            machine.occupied = true;
            return machine;

        }
    }
}