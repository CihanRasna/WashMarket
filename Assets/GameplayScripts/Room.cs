using System;
using System.Collections.Generic;
using System.Linq;
using GameplayScripts.Machines;
using RSNManagers;
using UnityEngine;
using UnityEngine.AI;

namespace GameplayScripts
{
    public class Room : MonoBehaviour , ISaveable<object>
    {
        public bool isActiveRoom;
        [SerializeField] private List<GameObject> walls;
        [SerializeField] private List<Room> neighborRooms = new(4);
        [SerializeField] private List< Machine> currentMachines;
        [SerializeField] private NavMeshSurface meshSurface;
        [SerializeField] public string uniqueID;
        

        public NavMeshData GetNavmeshData()
        {
            return meshSurface.navMeshData;
        }
        public void BuildNavMeshData()
        {
            meshSurface.enabled = true;
            meshSurface.BuildNavMesh();
        }

        public void RemoveNavMeshData()
        {
            meshSurface.navMeshData = null;
        }

        public void GetUniqueID(string id)
        {
            uniqueID = id;
        }

        public void CheckIfActiveRoom(bool isActiveRoom)
        {
            this.isActiveRoom = isActiveRoom;
            gameObject.SetActive(isActiveRoom);
        }

        public void GetNeighborRooms(Room neighborRoom)
        {
            if (!neighborRooms.Contains(neighborRoom))
            {
                neighborRooms.Add(neighborRoom);
            }

            if (neighborRoom.isActiveRoom)
            {
                var orderByDist = walls.OrderBy(w =>
                    Vector3.Distance(neighborRoom.transform.position, w.transform.position));
                var asd = orderByDist.First();
                var dist = Vector3.Distance(asd.transform.position, neighborRoom.transform.position);
                if (dist <= meshSurface.GetComponent<MeshRenderer>().bounds.extents.magnitude)
                {
                    asd.gameObject.SetActive(false);
                }
            }
        }

        public object CaptureState()
        {
            var data = new SaveData
            {
                //roomID = roomUniqueID,
                //machines = new Type[currentMachines.Count],
                machineCount = currentMachines.Count
            };

            /*for (var i = 0; i < currentMachines.Count; i++)
            {
                var type = currentMachines[i].GetType();
                //data.machines[i] = type;

                //var machineTransform = m.transform;
                //data.machinePos[i] = machineTransform.position;
                //data.machineRotations[i] = machineTransform.rotation;
                //data.machineLevels[i] = (int)m.currentLevel;
            }*/

            return data;
        }

        public void RestoreState(object state)
        {
            var data = state;
            //var myData = (SaveData)state;

            for (var i = 0; i < 3; i++)
            {
                //var machineType = saveData.machines[i];
                //var asd = new GameObject("machine", machineType);
                //var tempMachine = saveData.machines[i];
                //var pos = saveData.machinePos[i];
                //var rot = saveData.machineRotations[i];
                //Instantiate(asd,transform);                
            }
        }
        
        [Serializable]
        public class SaveData
        {
            //[SerializeField] public string roomID;
            [SerializeField] public int machineCount;
            //[SerializeField] public Type[] machines;
        }
    }
}