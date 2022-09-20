using System;
using System.Collections.Generic;
using GameplayScripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace RSNManagers
{
    public class RoomManager : Singleton<RoomManager>
    {
        [SerializeField] private Room roomPrefab;
        [SerializeField] private Transform roomHolder;
        [SerializeField] private List<Room> roomsOnScene;
        [SerializeField] private List<Room> currentlyActiveRooms;
        [SerializeField] private int horizontalRoomCount = 3;
        [SerializeField] private int verticalRoomCount = 5;
        [SerializeField] private List<string> roomIDList;
        

        protected override void Awake()
        {
            base.Awake();
            //InstantiateRoomPrefabs();
        }

        protected override void Start()
        {
            base.Start();
            var activeRoomCount = PersistManager.Instance.ActiveRoomCount;
            ActivateRooms(activeRoomCount);
            FindNeighborRooms();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                ActivateRoomsDynamic();
            }
        }

        [Button]
        private void InstantiateRoomPrefabs()
        {
            var count = 1;
            string posName = null;
            var desiredPos = Vector3.zero;
            for (var i = 0; i < verticalRoomCount; i++)
            {
                desiredPos.z = i * 15f;
                for (var j = 0; j < horizontalRoomCount; j++)
                {
                    if (j % 2 == 0)
                    {
                        desiredPos.x = j * 7.5f;
                        posName = $"[0,{i + 1},1]";
                    }
                    else
                    {
                        desiredPos.x = (-j - 1) * 7.5f;
                        posName = $"[1,{i + 1},0]";
                    }

                    var room = Instantiate(roomPrefab, desiredPos, Quaternion.identity, roomHolder);
                    room.name = $"Room {posName}    Order : {count}";
                    room.GetUniqueID(roomIDList[count - 1]);
                    roomsOnScene.Add(room);
                    count += 1;
                }
            }
        }

        [Button]
        private void RemoveRoomPrefabs()
        {
            foreach (var r in roomsOnScene)
            {
                DestroyImmediate(r.gameObject);
            }
            roomsOnScene.Clear();
        }
        
        [Button]
        private void GenerateIDList()
        {
            var count = horizontalRoomCount * verticalRoomCount;
            var currentIDCount = roomIDList.Count;
            for (var i = currentIDCount; i < count; i++)
            {
                roomIDList.Add(GenerateID());
            }
        }

        #region SAVELOADID

        private string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }

        #endregion


        private void ActivateRooms(int activeRoomCount)
        {
            for (var i = 0; i < roomsOnScene.Count; i++)
            {
                var currentRoom = roomsOnScene[i];
                currentRoom.CheckIfActiveRoom(i < activeRoomCount);

                if (i == activeRoomCount -1)
                {
                    Debug.Log(currentRoom.gameObject.name);
                    currentRoom.BuildNavMeshData();
                }
                
                if (i < activeRoomCount)
                {
                    currentlyActiveRooms.Add(currentRoom);
                    currentRoom.gameObject.SetActive(true);
                }
            }
        }

        private void ActivateRoomsDynamic()
        {
            var activeRoomCount = PersistManager.Instance.ActiveRoomCount;
            if (activeRoomCount >= horizontalRoomCount * verticalRoomCount) return;
            
            activeRoomCount = PersistManager.Instance.ActiveRoomCount += 1;
            var currentActiveRooms = currentlyActiveRooms.Count;
            var lastRoom = currentlyActiveRooms[^1];
            var navmeshData = lastRoom.GetNavmeshData();
            Destroy(navmeshData);

            for (var i = currentActiveRooms; i < activeRoomCount; i++)
            {
                var currentRoom = roomsOnScene[i];
                if (i < activeRoomCount)
                {
                    currentlyActiveRooms.Add(currentRoom);
                }

                if (i == activeRoomCount - 1)
                {
                    currentRoom.BuildNavMeshData();
                }
            }

            lastRoom.RemoveNavMeshData();
            FindNeighborRooms();
        }

        private void FindNeighborRooms()
        {
            for (var i = 0; i < roomsOnScene.Count; i++)
            {
                var neighborArray = new float?[4];
                var currentRoom = roomsOnScene[i];
                for (var j = 0; j < roomsOnScene.Count; j++)
                {
                    if (j == i) continue;

                    var possibleNeighbor = roomsOnScene[j];
                    var dist = Vector3.Distance(currentRoom.transform.position, possibleNeighbor.transform.position);
                    for (var k = 0; k < neighborArray.Length; k++)
                    {
                        //Debug.Log(currentRoom.gameObject.name + " : ITERATED");
                        if (neighborArray[k] == null)
                        {
                            neighborArray[k] = dist;
                        }
                        else if (neighborArray[k].HasValue && dist <= neighborArray[k].Value)
                        {
                            neighborArray[k] = dist;
                        }
                        else
                        {
                            continue;
                        }

                        currentRoom.GetNeighborRooms(possibleNeighbor);
                    }
                }
            }
        }
    }
}