using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace GameplayScripts
{
    public class Room : MonoBehaviour
    {
        public bool isActiveRoom;
        [SerializeField] private List<GameObject> walls;
        [SerializeField] private List<Room> neighborRooms = new(4);
        [SerializeField] private NavMeshSurface _meshSurface;

        [SerializeField] private string uniqueID;

        public NavMeshData GetNavmeshData()
        {
            return _meshSurface.navMeshData;
        }
        public void BuildNavMeshData()
        {
            _meshSurface.enabled = true;
            _meshSurface.BuildNavMesh();
        }

        public void RemoveNavMeshData()
        {
            _meshSurface.navMeshData = null;
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
                if (dist <= _meshSurface.GetComponent<MeshRenderer>().bounds.extents.magnitude)
                {
                    asd.gameObject.SetActive(false);
                }
            }
        }
    }
}