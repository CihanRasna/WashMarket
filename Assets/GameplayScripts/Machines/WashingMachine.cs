using UnityEngine;

namespace GameplayScripts.Machines
{
    public class WashingMachine : Machine
    {
        private static readonly int CoverOpen = Animator.StringToHash("CoverOpen");
        private static readonly int CoverClosed = Animator.StringToHash("CoverClosed");
        private static readonly int IsWorking = Animator.StringToHash("IsWorking");

        public override void Sell(out int price)
        {
            base.Sell(out price);
            Manager.washingMachines.Remove(this);
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

        protected override void RepairBehaviourOverride()
        {
            animator.SetTrigger(CoverOpen);
            base.RepairBehaviourOverride();
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