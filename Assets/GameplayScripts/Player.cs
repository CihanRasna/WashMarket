using System;
using System.Collections;
using DG.Tweening;
using RSNManagers;
using UnityEngine;

namespace GameplayScripts
{
    public class Player : Actor
    {
        private static readonly int Blend = Animator.StringToHash("Blend");

        private void Start()
        {
            transform.position = PersistManager.Instance.PlayersLastPos;
        }

        private void OnApplicationQuit()
        {
            PersistManager.Instance.PlayersLastPos = transform.position;
        }

        private void StartAAA()
        {
            var pos = Vector3.zero;
            var roomManager = RoomManager.Instance;
            var activeRooms = roomManager.ActiveRooms;
            var count = activeRooms.Count;
            for (var i = 0; i < count; i++)
            {
                pos += activeRooms[i].transform.position;
            }

            transform.position = pos / count;
        }

        public void Move(Vector2 direction)
        {
            var myTransform = transform;
            var targetForward = myTransform.forward;
            targetForward.x += direction.x;
            targetForward.z += direction.y;
            myTransform.forward = targetForward;
            myTransform.position += targetForward * (speed * direction.magnitude * Time.deltaTime);
            animator.SetFloat(Blend, direction.magnitude);
        }

        public void Stop()
        {
            transform.DOKill();
            var valueToLerp = animator.GetFloat(Blend);
            DOVirtual.Float(valueToLerp, 0f, 0.25f, OnVirtualUpdate).SetEase(Ease.InQuart);
        }

        private void OnVirtualUpdate(float v)
        {
            animator.SetFloat(Blend, v);
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