using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameplayScripts
{
    [CreateAssetMenu(fileName = "ClothType", menuName = "Cloth", order = 1)]
    public class CustomerItem : ScriptableObject
    {
        public bool needWash;
        public bool needDry;
        public bool needIron;
    }
}