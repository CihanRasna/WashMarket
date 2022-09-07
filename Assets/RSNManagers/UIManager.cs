using UnityEngine;

namespace RSNManagers
{
    public class UIManager : Singleton<UIManager>
    {
        public void OpenFailedPanel()
        {
            Debug.Log("failed");
        }
    }
}