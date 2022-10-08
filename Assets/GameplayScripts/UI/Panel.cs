using UnityEngine;

namespace GameplayScripts.UI
{
    public abstract class Panel : MonoBehaviour
    {
        [SerializeField] protected RectTransform rectTransform;
        public abstract void HidePanel();
    }
}
