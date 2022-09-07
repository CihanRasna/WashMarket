using UnityEngine;

namespace GameplayScripts
{
    public abstract class Machine : MonoBehaviour
    {
        public enum Level
        {
            Level1,
            Level2,
            Level3
        }

        public Level currentLevel;
        
        [SerializeField] private float workTime;
        [SerializeField] private float durability;
        [SerializeField] private float capacity;
        [SerializeField] private float consumption;
        [SerializeField] private Machine nextLevelMachine;
        
    }
}
