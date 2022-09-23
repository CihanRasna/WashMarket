using System;
using DG.Tweening;
using UnityEngine;

namespace GameplayScripts
{
    public class Draggable : MonoBehaviour
    {
        public LayerMask unplaceableLayers;
        public GameObject dummyGameObject;
        public Material dummyMaterial;
        public Transform machineObject;

        public void GetLayerMask(LayerMask layerMask)
        {
            unplaceableLayers = layerMask;
            CreateDummyMesh();
        }

        private bool _canPlace = false;
        public bool CanPlace => _canPlace;

        private void OnTriggerEnter(Collider other)
        {
            var wrongLayer = (unplaceableLayers.value & (1 << other.gameObject.layer)) > 0;
            if (wrongLayer)
            {
                _canPlace = false;
                dummyMaterial.DOColor(Color.red, 0.2f);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var wrongLayer = (unplaceableLayers.value & (1 << other.gameObject.layer)) > 0;
            if (wrongLayer)
            {
                _canPlace = true;
                dummyMaterial.DOColor(Color.green, 0.2f);
            }
        }

        public void Placed()
        {
            Destroy(dummyGameObject);
            DOTween.Kill(this);
            machineObject.DOScale(1f, 0.4f);
            machineObject.DOLocalMoveY(0f, 0.5f).OnComplete((() =>
            {
                Destroy(this);
            })).SetEase(Ease.OutBounce);
        }

        public void GetMachineMeshObject(Transform meshObject)
        {
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
            dummyGameObject.transform.Rotate(Vector3.right * 90f);
            dummyMaterial = dummyGameObject.GetComponent<Renderer>().material;
        }
    }
}