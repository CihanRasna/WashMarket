using System.Collections.Generic;
using UnityEngine;

public class TestSave : MonoBehaviour
{
    [SerializeField] private int myInt;
    [SerializeField] private List<GameObject> myMachines;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ES3.Save($"mymachines", myMachines);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            var asd = ES3.Load(gameObject.name, new List<GameObject>());
            for (int i = 0; i < asd.Count; i++)
            {
                var machine = asd[i];
                if (machine == null)
                {
                    machine = new GameObject();
                }
            }
        }
    }
}
