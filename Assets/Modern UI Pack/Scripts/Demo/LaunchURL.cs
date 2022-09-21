using UnityEngine;

namespace Modern_UI_Pack.Scripts.Demo
{
    public class LaunchURL : MonoBehaviour
    {
        public void GoToURL(string URL)
        {
            Application.OpenURL(URL);
        }
    }
}