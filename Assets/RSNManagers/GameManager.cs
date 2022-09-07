using System.Collections.Generic;
using Cinemachine;
using GameplayScripts;
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
        
        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
            LoadGame();
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
    }
}