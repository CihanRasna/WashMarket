using System.Collections.Generic;
using UnityEngine;

namespace GameplayScripts
{
    public class ClothPicker : MonoBehaviour
    {
        [SerializeField] public List<CustomerItem> clothTypes;

        public CustomerItem PickACustomerItem()
        {
            var clothTypesCount = clothTypes.Count;
            var rndOrder = Random.Range(0, clothTypesCount);
            return clothTypes[rndOrder];
        }
    }
}
