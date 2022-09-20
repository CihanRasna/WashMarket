using UnityEngine;

namespace RSNManagers
{
    public class PersistManager : Singleton<PersistManager>
    {
        private const string CurrencyKey = "Currency";
        private int _currency;

        public int Currency
        {
            get => _currency;
            set
            {
                _currency = value;
                PlayerPrefs.SetInt(CurrencyKey, _currency);
            }
        }

        private const string ActiveRoomKey = "ActiveRoomCount";
        private int _activeRoomCount;

        public int ActiveRoomCount
        {
            get => _activeRoomCount;
            set
            {
                _activeRoomCount = value;
                PlayerPrefs.SetInt(ActiveRoomKey, _activeRoomCount);
            }
        }
        
        public Vector3 PlayersLastPos
        {
            get
            {
                var x = PlayerPrefs.GetFloat("PlayerX", 0);
                var y = PlayerPrefs.GetFloat("PlayerY", 0);
                var z = PlayerPrefs.GetFloat("PlayerZ", 0);
                return new Vector3(x, y, z);
            }
            set
            {
                PlayerPrefs.SetFloat("PlayerX", value.x);
                PlayerPrefs.SetFloat("PlayerY", value.x);
                PlayerPrefs.SetFloat("PlayerZ", value.x);
            }
        }

        protected override void Awake()
        {
            Currency = PlayerPrefs.GetInt(CurrencyKey, 0);
            ActiveRoomCount = PlayerPrefs.GetInt(ActiveRoomKey, 1);
        }
    }
}