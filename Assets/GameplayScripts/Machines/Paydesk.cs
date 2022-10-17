using System.Collections;
using System.Collections.Generic;
using GameplayScripts.Characters;
using UnityEngine;

namespace GameplayScripts.Machines
{
    public class Paydesk : Machine
    {
        public Worker clerk;
        private Dictionary<int, Transform> CustomerQueuePositions { get; set; }

        [SerializeField] private List<Customer> customers;
        [SerializeField] private List<Transform> queuePositions;
        
        public override void Sell(out int price)
        {
            base.Sell(out price);
            Manager.payDesks.Remove(this);
            Manager.CheckForActiveMachineTypes();
            if (clerk is Clerk myClerk)
            {
                myClerk.LookingForWorkPlace();
            }
            Destroy(gameObject);
        }

        private bool _inPayment = false;

        protected override void Start()
        {
            navMeshObstacle.enabled = obstacleEnabled;
            CustomerQueuePositions = new Dictionary<int, Transform>(5);
            
            for (var i = 0; i < queuePositions.Count; i++)
            {
                CustomerQueuePositions[i] = queuePositions[i];
            }
        }

        public void AddCustomerToQueue(Customer customer)
        {
            if (customers.Count < 5)
            {
                occupied = false;
                if (!customers.Contains(customer))
                {
                    customers.Add(customer);
                }

                CustomerQueuePositions.TryGetValue(customers.IndexOf(customer), out var targetTransform);
                customer.GoToPaymentQueuePosition(targetTransform.position);
            }
            else
            {
                occupied = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var currentClerk = other.GetComponentInParent<Worker>();
            if (!clerk && currentClerk)
            {
                clerk = currentClerk;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var currentClerk = other.GetComponentInParent<Worker>();
            if (clerk && currentClerk)
            {
                clerk = null;
            }
        }

        private void Update()
        {
            occupied = customers.Count >= 5;
            if (clerk && customers.Count > 0 && !_currentCustomer && !_inPayment && customers[0].StateReadonly == Customer.State.Payment)
            {
                StartCoroutine(StartPaymentSequence());
            }
        }

        private IEnumerator StartPaymentSequence()
        {
            _inPayment = true;
            _currentCustomer = customers[0];
            yield return new WaitForSeconds(singleWorkTime);
            _currentCustomer.PaymentDone();
            customers.Remove(_currentCustomer);

            _currentCustomer = null;
            _inPayment = false;

            for (var i = 0; i < customers.Count; i++)
            {
                var customer = customers[i];
                AddCustomerToQueue(customer);
            }
        }

        public override void StartInteraction()
        {
        }

        public override void CurrentlyWorking()
        {
        }

        public override void FinishInteraction()
        {
        }
    }
}