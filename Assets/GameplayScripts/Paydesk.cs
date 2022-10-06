using System.Collections;
using System.Collections.Generic;
using RSNManagers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameplayScripts
{
    public class Paydesk : Machine
    {
        [SerializeField] private Actor clerk;
        private Dictionary<int, Transform> CustomerQueuePositions { get; set; }

        [SerializeField] private List<Customer> customers;

        private bool _inPayment = false;

        protected override void Start()
        {
            navMeshObstacle.enabled = obstacleEnabled;
            CustomerQueuePositions = new Dictionary<int, Transform>(5);
            const float offset = 1f;

            for (var i = 0; i < 5; i++)
            {
                var pos = new Vector3(.3f, 0, ((i + 1) * offset));
                var posHandler = new GameObject
                {
                    transform =
                    {
                        name = $"[{i}]. Slot",
                        parent = transform,
                        localPosition = pos
                    }
                };
                CustomerQueuePositions[i] = posHandler.transform;
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
            var currentClerk = other.GetComponentInParent<Player>();
            if (currentClerk)
            {
                clerk = currentClerk;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var currentClerk = other.GetComponentInParent<Player>();
            if (currentClerk)
            {
                clerk = null;
            }
        }

        private void Update()
        {
            occupied = customers.Count >= 5;
            if (clerk && !_currentCustomer && !_inPayment && customers.Count > 0)
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