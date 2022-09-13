using System;
using UnityEngine;

namespace GameplayScripts
{
    public class WashingMachine : Machine
    {
        private static readonly int CoverOpen = Animator.StringToHash("CoverOpen");
        private static readonly int CoverClosed = Animator.StringToHash("CoverClosed");
        private static readonly int IsWorking = Animator.StringToHash("IsWorking");

        private void Update()
        {
            if (occupied && Filled)
            {
               Working();
            }
        }

        public override void StartInteraction()
        {
            animator.SetTrigger(CoverOpen);
        }

        public override void CurrentlyWorking()
        {
            animator.SetBool(IsWorking, occupied && Filled);
        }

        public override void FinishInteraction()
        {
            animator.SetTrigger(CoverClosed);
        }
    }
}