using System.Collections;
using System.Collections.Generic;
using RSNManagers;
using UnityEngine;
using UnityEngine.AI;

namespace GameplayScripts
{
    public class Customer : Actor
    {
        private enum State
        {
            Idle,
            WaitingForMachine,
            LookingForMachine,
            GoingForMachine,
            FillMachine,
            Patrol,
            GoingForItems,
            GatheringItems,
            DudeGoingHome,
            Payment
        }

        public enum WorkType
        {
            Wash,
            Dry,
            Iron,
            Pay
        }

        [SerializeField] private WorkType workType;

        [SerializeField] private State state = State.Idle;
        private Vector3 _targetPosition;
        private Machine _currentlyUsingMachine;

        private readonly WaitForSeconds _initWaitForSeconds = new(2f);
        private readonly WaitForSeconds _fillWaitForSeconds = new(3f);

        private GameManager _gameManager;
        [SerializeField] private CustomerItem _customerItem;

        private IEnumerator Start()
        {
            yield return _initWaitForSeconds;
            _gameManager = GameManager.Instance;
            _customerItem = Instantiate(_gameManager.clothPicker.PickACustomerItem());

            if (state == State.Idle)
            {
                state = State.LookingForMachine;
            }

            if (state == State.LookingForMachine)
            {
                StartCoroutine(LookingForFreeMachine());
            }
        }

        private WorkType CheckClothesWorkType()
        {
            if (_customerItem.needWash)
            {
                return workType = WorkType.Wash;
            }

            if (_customerItem.needDry)
            {
                return workType = WorkType.Dry;
            }

            if (_customerItem.needIron)
            {
                return workType = WorkType.Iron;
            }
                
            return workType = WorkType.Pay;
            
        }

        private IEnumerator LookingForFreeMachine()
        {
            var clothesWorkType = CheckClothesWorkType();
            _currentlyUsingMachine = _gameManager.FindClosestMachine(clothesWorkType, transform);

            if (_currentlyUsingMachine)
            {
                var targetForward = _currentlyUsingMachine.transform;
                _targetPosition = targetForward.position + targetForward.forward;
                agent.destination = _targetPosition;

                if (workType != WorkType.Pay)
                {
                    _currentlyUsingMachine.MachineIsOccupied(this);
                    state = State.GoingForMachine;
                    yield break;
                }
                else
                {
                    state = State.Payment;
                    GoToPay();
                    yield break;
                }
            }

            state = State.WaitingForMachine;
            yield return _initWaitForSeconds;
            StartCoroutine(LookingForFreeMachine());
        }

        private void Update()
        {
            if (state == State.GoingForMachine)
            {
                var dist = agent.remainingDistance;
                if (agent.pathStatus == NavMeshPathStatus.PathComplete && dist == 0)
                {
                    state = State.FillMachine;
                    StartCoroutine(StartFillRoutine());
                }
            }

            if (state == State.GoingForItems)
            {
                var dist = agent.remainingDistance;
                if (agent.pathStatus == NavMeshPathStatus.PathComplete && dist == 0)
                {
                    state = State.GatheringItems;
                    StartCoroutine(StartEmptyingRoutine());
                }
            }

            if (state == State.Patrol)
            {
                /*var dist = agent.remainingDistance;
                if (agent.pathStatus == NavMeshPathStatus.PathComplete && dist == 0)
                {
                    agent.destination = RandomNavSphere(transform.position, 5f, -1);
                }*/
            }
        }

        private IEnumerator StartFillRoutine()
        {
            _currentlyUsingMachine.StartInteraction();
            yield return _fillWaitForSeconds;
            _currentlyUsingMachine.FinishInteraction();
            _currentlyUsingMachine.StartWork(this);
            state = State.Patrol;
            agent.destination = RandomNavSphere(transform.position, 5f, -1);
        }

        private IEnumerator StartEmptyingRoutine()
        {
            _currentlyUsingMachine.StartInteraction();
            yield return _fillWaitForSeconds;
            _currentlyUsingMachine.FinishInteraction();

            _currentlyUsingMachine.Empty();
            _currentlyUsingMachine = null;

            state = State.LookingForMachine;
            StartCoroutine(LookingForFreeMachine());
        }

        public void GoToPay()
        {
            var paydesk = (Paydesk)_currentlyUsingMachine;
            if (!paydesk.Customers.Contains(this))
            {
                paydesk.Customers.Enqueue(this);
            }

            var pos = paydesk.CustomerQueuePositions[paydesk.Customers.Count - 1];
            agent.destination = pos;
        }

        public void MachineFinished()
        {
            var clothesWorkType = CheckClothesWorkType();
            if (clothesWorkType == WorkType.Wash)
            {
                _customerItem.needWash = false;
            }
            else if (clothesWorkType == WorkType.Dry)
            {
                _customerItem.needDry = false;
            }
            else if (clothesWorkType == WorkType.Iron)
            {
                _customerItem.needIron = false;
            }

            state = State.GoingForItems;
            agent.destination = _targetPosition;
        }

        public void PaymentDone()
        {
            state = State.DudeGoingHome;
            agent.destination = _gameManager.leavePos.position;
        }

        private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            var randDirection = Random.insideUnitSphere * dist;

            randDirection += origin;

            var anan = NavMesh.SamplePosition(randDirection, out var navHit, dist, layermask);

            if (!anan)
            {
                RandomNavSphere(origin, dist, layermask);
            }

            return navHit.position;
        }
    }
}