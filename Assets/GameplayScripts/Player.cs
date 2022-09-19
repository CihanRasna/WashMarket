using System;
using RSNManagers;
using UnityEngine;

namespace GameplayScripts
{
    public class Player : Actor
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                SaveLoadManager.Instance.Save();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                SaveLoadManager.Instance.Load();
            }
        }

        public void Move(Vector2 direction)
        {
            var myTransform = transform;
            var targetForward = myTransform.forward;
            targetForward.x += direction.x;
            targetForward.z += direction.y;
            myTransform.forward = targetForward;
            myTransform.position += targetForward * (speed * direction.magnitude * Time.deltaTime);
        }
        
        
        
        [Serializable]
        public struct SaveData
        {
            [SerializeField] public string roomID;
            [SerializeField] public int machineCount;
            [SerializeField] public Type[] machines;
        }
    }
}