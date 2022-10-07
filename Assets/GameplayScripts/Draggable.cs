using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RSNManagers;
using UnityEngine;
using UnityEngine.AI;

namespace GameplayScripts
{
    public class Draggable : MonoBehaviour
    {
        private NavMeshObstacle navMeshObstacle;
        public LayerMask unplaceableLayers;
        public GameObject dummyGameObject;
        public Material dummyMaterial;
        public Machine machine;
        public Transform machineObject;
        public bool isRotating = false;
        private readonly List<GameObject> _collisionObjects = new List<GameObject>();
        private int _price;

        public void GetLayerMaskAndMeshData(LayerMask layerMask, NavMeshObstacle meshObstacle, int price = 0)
        {
            _price = price;
            navMeshObstacle = meshObstacle;
            unplaceableLayers = layerMask;
            navMeshObstacle.enabled = false;
            CreateDummyMesh();
        }

        private bool _canPlace = false;
        public bool CanPlace => _canPlace;

        private void OnTriggerEnter(Collider other)
        {
            _collisionObjects.Add(other.gameObject);
            CheckForPlacement();
        }

        private void OnTriggerExit(Collider other)
        {
            _collisionObjects.Remove(other.gameObject);
            CheckForPlacement();
        }

        private void CheckForPlacement()
        {
            var count = _collisionObjects.Count;
            for (var i = 0; i < count; i++)
            {
                var other = _collisionObjects[i];
                var wrongLayer = (unplaceableLayers.value & (1 << other.layer)) > 0;
                if (wrongLayer)
                {
                    _canPlace = false;
                    dummyMaterial.DOColor(Color.red, 0.2f);
                    return;
                }
            }

            _canPlace = true;
            dummyMaterial.DOColor(Color.green, 0.2f);
        }

        private Transform GetClosestRoom(List<Room> rooms)
        {
            Transform bestTarget = null;
            var closestDistanceSqr = Mathf.Infinity;
            var currentPosition = transform.position;

            foreach (var potentialTarget in rooms)
            {
                var directionToTarget = potentialTarget.transform.position - currentPosition;
                var dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget.transform;
                }
            }

            return bestTarget;
        }

        public void Placed()
        {
            var roomManager = RoomManager.Instance;
            var gameManager = GameManager.Instance;
            
            var roomList = roomManager.ActiveRooms;
            gameManager.CheckForActiveMachineTypes();
            transform.parent = GetClosestRoom(roomList);
            machine.obstacleEnabled = true;
            navMeshObstacle.enabled = true;
            Destroy(dummyGameObject);
            DOTween.Kill(this,true);
            machineObject.DOScale(1f, 0.4f);
            machineObject.DOLocalMoveY(0f, 0.5f).OnComplete((() =>
            {
                if (_price != 0)
                    PersistManager.Instance.Currency -= _price;
                Destroy(this);
            })).SetEase(Ease.OutBounce);
        }

        public void GetMachineMeshObject(Machine machine,Transform meshObject)
        {
            this.machine = machine;
            meshObject.transform.localPosition += Vector3.up;
            machineObject = meshObject;
            machineObject.DOShakeScale(0.5f, .2f, 5).SetLoops(-1, LoopType.Yoyo).SetId(this);
        }

        private void CreateDummyMesh()
        {
            var boxCollider = GetComponent<BoxCollider>();
            var size = boxCollider.size;
            var pos = boxCollider.center;
            var desiredSize = new Vector3(size.x, size.z, 1);
            var desiredPos = new Vector3(pos.x, 0.1f, pos.z);
            dummyGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(dummyGameObject.GetComponent<MeshCollider>());
            dummyGameObject.transform.parent = transform;
            dummyGameObject.transform.localPosition = desiredPos;
            dummyGameObject.transform.localScale = desiredSize;
            dummyGameObject.transform.DOLocalRotate(new Vector3(90f, 0f, 0f), 0f);
            dummyMaterial = dummyGameObject.GetComponent<Renderer>().material;
            dummyMaterial.color = Color.green;
        }
    }
}