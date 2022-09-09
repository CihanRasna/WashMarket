using System;
using System.Collections;
using System.Collections.Generic;
using RSNManagers;
using UnityEngine;

namespace GameplayScripts
{
    public class Paydesk : MonoBehaviour
    {
        [SerializeField] private Customer currentCustomer;
        [SerializeField] private Actor clerk;

        [SerializeField] private List<Vector3> customerQueuePositions = new(5);
        public List<Vector3> CustomerQueuePositions => customerQueuePositions;
        public Queue<Customer> Customers;

        private bool _inPayment = false;

        private void Start()
        {
            GameManager.Instance.paydesk = this;
            Customers = new Queue<Customer>(5);
            var localPosition = transform.localPosition;
            var forward = localPosition + transform.forward;
            var isBackward = forward.z <= 0 ? 1 : -1;

            for (var i = 0; i < 5; i++)
            {
                var offset = 1.5f * isBackward;
                forward.z += offset;
                customerQueuePositions.Add(forward);
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
            if (clerk && !currentCustomer && !_inPayment && Customers.Count > 0)
            {
                StartCoroutine(StartPaymentSequence());
            }
        }

        private IEnumerator StartPaymentSequence()
        {
            _inPayment = true;
            currentCustomer = Customers.Peek();
            yield return new WaitForSeconds(3f);
            currentCustomer.PaymentDone();
            Customers.Dequeue();
            foreach (var customer in Customers)
            {
                customer.GoToPay();
            }
            currentCustomer = null;
            _inPayment = false;
        }
    }
}