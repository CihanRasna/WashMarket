using System.Collections.Generic;
using UnityEngine;

namespace GameplayScripts.UI
{
    public class LoadGamePanel : Panel
    {
        [SerializeField] private List<SaveSlot> loadSlots;
        
        public void OpenLoadGamePanel()
        {
            gameObject.SetActive(true);
        }

        private void LoadDataIfExist()
        {
            
        }

        public override void HidePanel()
        {
            gameObject.SetActive(false);
        }
    }
}
