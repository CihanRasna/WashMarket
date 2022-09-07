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

        protected override void Awake()
        {
            Currency = PlayerPrefs.GetInt(CurrencyKey, 0);
            ActiveRoomCount = PlayerPrefs.GetInt(ActiveRoomKey, 1);
        }
    }
}