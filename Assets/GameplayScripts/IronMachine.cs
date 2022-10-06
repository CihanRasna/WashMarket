using UnityEngine;

namespace GameplayScripts
{
    public class IronMachine : Machine
    {
        private static readonly int IsWorking = Animator.StringToHash("IsWorking");

        public override void Sell(out int price)
        {
            base.Sell(out price);
            Manager.allMachines.Remove(this);
            Manager.ironMachines.Remove(this);
            Manager.CheckForActiveMachineTypes();
            Destroy(gameObject);
        }

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