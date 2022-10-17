using System;
using System.Collections.Generic;
using DG.Tweening;
using GameplayScripts.Machines;
using UnityEngine;

namespace GameplayScripts.Characters
{
    public class Clerk : Worker
    {
        [SerializeField] private Paydesk workingPlace;

        private float _passedTime;

        protected override void Start()
        {
            base.Start();
            workerType = WorkerType.Clerk;
            if (state == State.Idle)
            {
                LookingForWorkPlace();
            }
        }

        private void LookingForWorkPlace()
        {
            Paydesk machine = null;
            var desks = new List<Paydesk>(GameManager.payDesks);

            var closestDist = float.PositiveInfinity;

            for (var i = 0; i < desks.Count; i++)
            {
                var currentMachine = desks[i];
                var machineTransform = currentMachine.transform;
                if (!currentMachine.clerk)
                {
                    var dist = (transform.position - machineTransform.position).sqrMagnitude;
                    if (dist <= closestDist)
                    {
                        closestDist = dist;
                        machine = currentMachine;
                    }
                }
            }

            if (machine)
            {
                state = State.GoingForWork;
                workingPlace = machine;
                agent.destination = workingPlace.transform.position;
                workingPlace.clerk = this;
            }
            else
            {
                Debug.Log("NO PLACE TO WORK");
                agent.destination = GameManager.CalculateRandomPoint();
                // TODO: ALERT NO PLACE TO WORK VFX
            }
        }

        private void Update()
        {
            animator.SetFloat(Blend, agent.velocity.magnitude);
            
            _passedTime += Time.deltaTime;
            if (!workingPlace)
            {
                if (_passedTime >= 30f)
                {
                    _passedTime = 0f;
                    LookingForWorkPlace();
                }
            }

            if (state == State.GoingForWork && AgentIsArrived())
            {
                state = State.Working;
                transform.DOLookAt(workingPlace.transform.forward, 0.35f, AxisConstraint.Y);
            }
        }
    }
}