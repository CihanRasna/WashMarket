using System.Collections;
using GameplayScripts.Cloth;
using GameplayScripts.Machines;
using RSNManagers;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace GameplayScripts.Characters
{
    [SelectionBase]
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
        public State StateReadonly => state;
        
        private Vector3 _targetPosition;
        private Machine _currentlyUsingMachine;

        private readonly WaitForSeconds _initWaitForSeconds = new(1f);
        private readonly WaitForSeconds _fillWaitForSeconds = new(3f);

        private GameManager _gameManager;
        [SerializeField] private CustomerItem _customerItem;
        [SerializeField] private int _workCost;

        private float _customerSessionTime;
        private static readonly int CarryWalk = Animator.StringToHash("CarryWalk");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Speed = Animator.StringToHash("Speed");

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
            
            Debug.Log("SPAWNED");
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

        public void MachineBroke()
        {
            _currentlyUsingMachine = null;
            animator.SetTrigger(Walk);
            state = State.DudeGoingHome;
            agent.destination = _gameManager.leavePos.position;
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

            animator.SetTrigger(Walk);
            agent.destination = _targetPosition;
            state = State.GoingForItems;
        }

        private IEnumerator LookingForFreeMachine()
        {
            var clothesWorkType = CheckClothesWorkType();
            _currentlyUsingMachine = _gameManager.FindClosestMachine(clothesWorkType, transform);

            if (_currentlyUsingMachine)
            {
                animator.SetTrigger(CarryWalk);
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
                    var paydesk = (Paydesk)_currentlyUsingMachine;
                    paydesk.AddCustomerToQueue(this);
                    yield break;
                }
            }

            animator.SetTrigger(CarryWalk);
            state = State.WaitingForFreeMachine;
            agent.destination = _gameManager.CalculateRandomPoint();
            yield return _initWaitForSeconds;
            StartCoroutine(LookingForFreeMachine());
        }

        private void Update()
        {
            animator.SetFloat(Speed, agent.velocity.normalized.magnitude);

            if (state == State.WaitingForFreeMachine)
            {
                _customerSessionTime += Time.deltaTime;
                if (_customerSessionTime >= 30f)
                {
                    ShopClosed(); // HAVE TO CHANGE TO CUSTOMER BEHAVIOUR AGAINST WAITING
                }
            }

            if (state == State.GoingForMachine && AgentIsArrived())
            {
                state = State.FillMachine;
                StartCoroutine(StartFillRoutine());
            }

            if (state == State.GoingForItems && AgentIsArrived())
            {
                state = State.GatheringItems;
                StartCoroutine(StartEmptyingRoutine());
            }

            if (state == State.GoingForPayment && AgentIsArrived())
            {
                //var paydesk = (Paydesk)_currentlyUsingMachine;
                //paydesk.AddCustomerToQueue(this);
                state = State.Payment;
            }

            if (state == State.DudeGoingHome && AgentIsArrived())
            {
                Destroy(gameObject);
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
            animator.SetTrigger(Walk);
            
            agent.destination = _gameManager.CalculateRandomPoint();
            //agent.destination = RandomNavSphere(transform.position, 5f, -1); //_gameManager.leavePos.position; //;
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

        public void PaymentDone()
        {
            Debug.Log("PAY");
            animator.SetTrigger(CarryWalk);
            PersistManager.Instance.Currency += _workCost;
            state = State.DudeGoingHome;
            agent.destination = _gameManager.leavePos.position;
        }

        public void ShopClosed()
        {
            var available = state is State.Idle or State.WaitingForFreeMachine or State.LookingForFreeMachine
                or State.Payment;

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

        private bool AgentIsArrived()
        {
            return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance &&
                   (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(agent.destination,0.2f);
        }
    }
}