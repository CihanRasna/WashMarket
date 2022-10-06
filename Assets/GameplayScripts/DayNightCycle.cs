using System;
using DG.Tweening;
using RSNManagers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayScripts
{
    public class DayNightCycle : MonoBehaviour
    {
        public event Action<float> ShiftStartedAction;
        public event Action ShiftEndedAction;
        
        [SerializeField] private Image minuteHand;
        [SerializeField] private Image hourHand;
        [SerializeField] private Image clockBackground;
        [SerializeField] private TextMeshProUGUI shiftTMP;

        [SerializeField] private float dayTimeInSecond;
        [SerializeField] private float shiftTimeInSecond;

        [SerializeField] private float closedTimeSlowerMultiplier = 3f;
        

        //[SerializeField] private float dayTimeInRealTimeMinute;
        //[SerializeField] private float shiftTimeInRealtimeMinute;

        [SerializeField] private bool inShiftTime;
        [SerializeField] private int passedDayCount;

        private bool _firstDayOfSession = false;

        private float _passedTime;
        private float _normalizeShiftValue;

        private bool _skipping;

        private void Start()
        {
            if (transform.parent) transform.SetParent(null,true);
            DontDestroyOnLoad(gameObject);
            passedDayCount = PersistManager.Instance.PassedDayCount;
            _normalizeShiftValue = shiftTimeInSecond / dayTimeInSecond;

            shiftTMP.text = $"OPEN SHOP";
        }

        private void ClockTimeTweener(float dayTime, bool inShift)
        {
            DOTween.Kill(targetOrId: 2709);

            inShiftTime = inShift;

            var rotateAmount = new Vector3(0, 0, 360);

            if (inShift)
            {
                ShiftStartedAction.Invoke(shiftTimeInSecond);
                //rotateAmount = new Vector3(0, 0, 360);
                clockBackground.fillClockwise = true;
                clockBackground.DOFillAmount(_normalizeShiftValue, 0.5f);
                clockBackground.DOColor(new Color(0, 255, 0, 0.6667f), 0.5f);
            }
            else
            {
                clockBackground.fillClockwise = false;
                clockBackground.DOFillAmount(1f - _normalizeShiftValue, 0.5f);
                clockBackground.DOColor(new Color(255, 0, 0, 0.6667f), 0.5f);
            }

            var calculatedMinuteTime = dayTime / 24f;

            hourHand.rectTransform.DOLocalRotate(rotateAmount, dayTime, RotateMode.LocalAxisAdd).SetLoops(-1)
                .SetEase(Ease.Linear).SetId(2709);
            minuteHand.rectTransform.DOLocalRotate(rotateAmount, calculatedMinuteTime, RotateMode.LocalAxisAdd).SetLoops(-1)
                .SetEase(Ease.Linear).SetId(2709);
        }

        public void PointerHover(bool onHover)
        {
            if (!_firstDayOfSession)
            {
                shiftTMP.text = $"OPEN SHOP";
                return;
            }

            if (_skipping)
            {
                shiftTMP.text = $"SKIPPING";
                return;
            }

            if (!inShiftTime)
            {
                shiftTMP.text = onHover ? $"FAST FORWARD" : $"CLOSED";
            }
        }

        private void CloseTime()
        {
            inShiftTime = false;
            passedDayCount += 1;
            _passedTime = 0f;
            shiftTMP.text = $"CLOSED";
            
            ShiftEndedAction.Invoke();
            
            PersistManager.Instance.PassedDayCount = passedDayCount;

            StartOrSkipDay();
        }

        public void StartOrSkipDay(bool isButtonClick = false)
        {
            if (!_firstDayOfSession)
            {
                shiftTMP.text = $"WORK HOURS";
                _firstDayOfSession = true;
                ClockTimeTweener(dayTimeInSecond, true);
                return;
            }

            if (!inShiftTime && !_skipping)
            {
                DOTween.Kill(targetOrId: 2709);

                var hourHandEuler = hourHand.rectTransform.eulerAngles;
                var hourTargetRelativeRotate = new Vector3(0, 0, 270f - hourHandEuler.z);

                float hourHandDuration;
                float minuteHandDuration;

                if (isButtonClick)
                {
                    _skipping = true;
                    shiftTMP.text = $"SKIPPING";
                    hourHandDuration = 4f;
                    minuteHandDuration = 3f;
                    var minuteHandEuler = minuteHand.rectTransform.eulerAngles;
                    var minuteTargetRelativeRotate = new Vector3(0, 0, 360f - minuteHandEuler.z);
                    minuteHand.rectTransform
                        .DOLocalRotate(minuteTargetRelativeRotate, minuteHandDuration, RotateMode.LocalAxisAdd)
                        .SetRelative().SetEase(Ease.Linear).SetId(2709);
                }
                else
                {
                    hourHandDuration = dayTimeInSecond * closedTimeSlowerMultiplier;
                    minuteHandDuration = hourHandDuration / 24f;
                    var rotateAmount = new Vector3(0, 0, 360);

                    minuteHand.rectTransform.DOLocalRotate(rotateAmount, minuteHandDuration, RotateMode.LocalAxisAdd)
                        .SetLoops(-1)
                        .SetEase(Ease.Linear).SetId(2709);
                }

                hourHand.rectTransform.DOLocalRotate(hourTargetRelativeRotate, hourHandDuration, RotateMode.LocalAxisAdd)
                    .SetRelative()
                    .SetEase(Ease.Linear).OnComplete((() =>
                    {
                        _skipping = false;
                        shiftTMP.text = $"WORK HOURS";
                        ClockTimeTweener(dayTimeInSecond, true);
                    }))
                    .SetId(2709);
            }
        }

        private void Update()
        {
            if (inShiftTime)
            {
                _passedTime += Time.deltaTime;

                if (_passedTime >= shiftTimeInSecond)
                {
                    CloseTime();
                }
            }
        }
    }
}