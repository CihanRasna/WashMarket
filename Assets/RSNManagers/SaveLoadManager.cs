using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace RSNManagers
{
    public class SaveLoadManager : MonoBehaviour//Singleton<SaveLoadManager> NOT USING FOR NOW
    {
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
    }
}