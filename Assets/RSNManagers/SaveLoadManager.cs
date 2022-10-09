using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RSNManagers
{
    public class SaveLoadManager : Singleton<SaveLoadManager> 
    {
        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }

        protected void OnApplicationQuit()
        {
            base.OnDestroy();
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }

        private int _slotIdx;
        
        public void SaveData(int slotIdx)
        {
            _slotIdx = slotIdx;
            var pathString = $"SaveSlot{slotIdx.ToString()}.rsn";
            var key = $"{slotIdx.ToString()}.rsn";
            var autoSaves = ES3AutoSaveMgr.Current.autoSaves;
            var gameObjects = new List<GameObject>();
            foreach (var autoSave in autoSaves)
            {
                if (autoSave.enabled)
                    gameObjects.Add(autoSave.gameObject);
            }
            var settings = new ES3Settings(ES3.EncryptionType.None, "myPassword")
            {
                location = ES3.Location.File,
                path = pathString
            };
            
            ES3.Save<GameObject[]>(key, gameObjects.ToArray(), settings);
        }

        private void OnLevelFinishedLoading(Scene arg0, LoadSceneMode arg1)
        {
            Debug.Log("ANAN");
            var pathString = $"SaveSlot{_slotIdx.ToString()}.rsn";
            var key = $"{_slotIdx.ToString()}.rsn";
            var settings = new ES3Settings(ES3.EncryptionType.None, "myPassword")
            {
                location = ES3.Location.File,
                path = pathString
            };
            ES3.Load<GameObject[]>(key, Array.Empty<GameObject>(), settings);
        }

        public void LoadData(int slotIdx)
        {
            _slotIdx = slotIdx;
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadSceneAsync(scene.name, LoadSceneMode.Single);
        }
    }
    
    /*#region OldSaveManager

        private const string SaveDirectory = "/SaveData/";
        private const string FileName = "SaveGame.rsn";

        public void Save()
        {
            var state = LoadFile();
            CaptureState(state);
            SaveFile(state);
        }

        public void Load()
        {
            var state = LoadFile();
            RestoreState(state);
        }

        private void SaveFile(object state)
        {
            var dir = $"{Application.persistentDataPath}{SaveDirectory}";

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonConvert.SerializeObject(state, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                });
            File.WriteAllText($"{dir}{FileName}", json);
            GUIUtility.systemCopyBuffer = $"{dir}{FileName}";
        }

        private Dictionary<string, object> LoadFile()
        {
            var dir = $"{Application.persistentDataPath}{SaveDirectory}";
            if (!File.Exists($"{dir}{FileName}"))
            {
                Debug.Log("LO");
                return new Dictionary<string, object>();
            }

            var json = File.ReadAllText($"{dir}{FileName}");
            var deserializeObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            return deserializeObject;
        }

        private void CaptureState(IDictionary<string, object> state)
        {
            var saveables = RoomManager.Instance.RoomsOnScene;

            foreach (var saveable in saveables)
            {
                state[saveable.uniqueID] = saveable.CaptureState();
            }
        }

        private void RestoreState(IReadOnlyDictionary<string, object> state)
        {
            var saveables = RoomManager.Instance.ActiveRooms;

            foreach (var saveable in saveables)
            {
                if (state.TryGetValue(saveable.uniqueID, out var value))
                {
                    saveable.RestoreState(value);
                }
            }
        }

        #endregion*/
}