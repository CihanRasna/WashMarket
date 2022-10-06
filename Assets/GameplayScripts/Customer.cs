using System.Collections;
using RSNManagers;
using UnityEngine;
using UnityEngine.AI;

namespace GameplayScripts
{
    public class Customer : Actor
    {
        public enum State
        {
            Idle,
            WaitingForFreeMachine,
            LookingForFreeMachine,
            GoingForMachine,
            FillMachine,
            Patrol,
            GoingForItems,
            GatheringItems,
            GoingForPayment,
            Payment,
            DudeGoingHome
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

        private readonly WaitForSeconds _initWaitForSeconds = new(1f);
        private readonly WaitForSeconds _fillWaitForSeconds = new(3f);

        private GameManager _gameManager;
        [SerializeField] private CustomerItem _customerItem;
        [SerializeField] private int _workCost;

        private float _customerSessionTime;

        private IEnumerator Start()
        {
            yield return _initWaitForSeconds;
            _gameManager = GameManager.Instance;
            var vertices = _gameManager.waitingAreaDemo.mesh.vertices;
            agent.destination = vertices[Random.Range(0, vertices.Length)];
            _customerItem = Instantiate(_gameManager.clothPicker.PickACustomerItem());

            if (state == State.Idle)
            {
                state = State.LookingForFreeMachine;
            }

            if (state == State.LookingForFreeMachine)
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
            
            Debug.Log(_currentlyUsingMachine);
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
                    state = State.GoingForPayment;
                    //var paydesk = (Paydesk)_currentlyUsingMachine;
                    //paydesk.AddCustomerToQueue(this);
                    yield break;
                }
            }

            state = State.WaitingForFreeMachine;
            var vertices = _gameManager.waitingAreaDemo.mesh.vertices;
            agent.destination = vertices[Random.Range(0, vertices.Length)];
            yield return _initWaitForSeconds;
            StartCoroutine(LookingForFreeMachine());
        }

        private void Update()
        {
            if (state == State.WaitingForFreeMachine)
            {
                _customerSessionTime += Time.deltaTime;
                if (_customerSessionTime >= 30f)
                {
                    ShopClosed(); // HAVE TO CHANGE TO CUSTOMER BEHAVIOUR AGAINST WAITING
                }
            }
            
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

            if (state == State.GoingForPayment)
            {
                var dist = agent.remainingDistance;
                if (agent.pathStatus == NavMeshPathStatus.PathComplete && dist == 0)
                {
                    var paydesk = (Paydesk)_currentlyUsingMachine;
                    paydesk.AddCustomerToQueue(this);
                    state = State.Payment;
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
            _workCost += _currentlyUsingMachine.UsingPrice;
            state = State.Patrol;
            agent.destination = RandomNavSphere(transform.position, 5f, -1); //_gameManager.leavePos.position; //;
        }

        public void GoToPaymentQueuePosition(Vector3 pos)
        {
            state = State.GoingForPayment;
            agent.destination = pos;
        }

        private IEnumerator StartEmptyingRoutine()
        {
            _currentlyUsingMachine.StartInteraction();
            yield return _fillWaitForSeconds;
            _currentlyUsingMachine.FinishInteraction();

            _currentlyUsingMachine.Empty();
            _currentlyUsingMachine = null;

            state = State.LookingForFreeMachine;
            StartCoroutine(LookingForFreeMachine());
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
            PersistManager.Instance.Currency += _workCost;
            state = State.DudeGoingHome;
            agent.destination = _gameManager.leavePos.position;
        }

        public void ShopClosed()
        {
            var available = state is State.Idle or State.WaitingForFreeMachine or State.LookingForFreeMachine or State.Payment;
            
            if (available)
            {
                state = State.DudeGoingHome;
                agent.destination = _gameManager.leavePos.position;
                _gameManager.ShiftEndedAction -= ShopClosed;
            }
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