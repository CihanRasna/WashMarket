using System;
using System.Collections.Generic;
using UnityEngine;

namespace RSNManagers
{
    public class SaveableEntity : MonoBehaviour
    {
        /*
        public string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }

        public object CaptureState()
        {
            var state = new Dictionary<string, object>();
            var saveables = new List<ISaveable>();

            foreach (var room in RoomManager.Instance.RoomsOnState)
            {
                saveables.Add(room.GetComponent<ISaveable>());
            }

            foreach (var saveable in saveables)
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }

            return state;
        }

        public void RestoreState(object state)
        {
            var stateDictionary = (Dictionary<string, object>)state;
            
            var saveables = new List<ISaveable>();

            foreach (var room in RoomManager.Instance.RoomsOnState)
            {
                saveables.Add(room.GetComponent<ISaveable>());
            }

            foreach (var saveable in saveables)
            {
                var typeName = saveable.GetType().ToString();
                if (stateDictionary.TryGetValue(typeName,out var value))
                {
                    saveable.RestoreState(value);
                }
            }
        }*/
    }
}