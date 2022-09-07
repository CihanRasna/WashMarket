using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameplayScripts
{
    public class Room : MonoBehaviour
    {
        public bool isActiveRoom;
        [SerializeField] private List<GameObject> walls;
        [SerializeField] private List<Room> neighborRooms = new(4);

        private int _roomID;
        private int _roomUniqueID;

        public void GetIDFromManager(int id, bool isActiveRoom)
        {
            this.isActiveRoom = isActiveRoom;
            _roomID = id;
            _roomUniqueID = $"{gameObject.name},{_roomID.ToString()}".GetHashCode();

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
                if (dist <= GetComponent<MeshRenderer>().bounds.extents.magnitude)
                {
                    asd.gameObject.SetActive(false);
                }
            }
        }
    }
}