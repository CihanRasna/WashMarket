using System.Collections.Generic;
using UnityEngine;

namespace GameplayScripts.Cloth
{
    public class ClothPicker : MonoBehaviour
    {
        [SerializeField] public List<CustomerItem> allClothTypes;
        
        [SerializeField] public List<CustomerItem> usableClothTypes;

        public CustomerItem PickACustomerItem()
        {
            var clothTypesCount = usableClothTypes.Count;
            var rndOrder = Random.Range(0, clothTypesCount);
            return usableClothTypes[rndOrder];
        }
    }
}
