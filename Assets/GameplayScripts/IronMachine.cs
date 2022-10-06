using UnityEngine;

namespace GameplayScripts
{
    public class IronMachine : Machine
    {
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
        }

        public override void CurrentlyWorking()
        {
            animator.SetBool(IsWorking, occupied && Filled);
        }

        public override void FinishInteraction()
        {
        }
    }
}