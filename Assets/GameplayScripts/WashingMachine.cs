using System;
using UnityEngine;

namespace GameplayScripts
{
    public class WashingMachine : Machine
    {
        private void Update()
        {
            if (occupied && Filled)
            {
               Working();
            }
        }
    }
}