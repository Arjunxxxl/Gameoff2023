using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class ButtonAnimator : MonoBehaviour
{
    [SerializeField] private Transform target;

    Vector3 originalScale;
    private bool isTouchDownAnimationComplete;

    private EventTrigger eventTrigger;

    private void Start()
    {
        if(target == null)
        {
            target = transform;
        }

        originalScale = target.localScale;

        eventTrigger = GetComponent<EventTrigger>();

        EventTrigger.Entry entryDown = new EventTrigger.Entry();
        entryDown.eventID = EventTriggerType.PointerDown;
        entryDown.callback.AddListener((eventData) => { AnimateTouchDown(); });
        eventTrigger.triggers.Add(entryDown);

        EventTrigger.Entry entryUp = new EventTrigger.Entry();
        entryUp.eventID = EventTriggerType.PointerUp;
        entryUp.callback.AddListener((eventData) => { AnimateTouchUp(); });
        eventTrigger.triggers.Add(entryUp);

        EventTrigger.Entry entryCancle = new EventTrigger.Entry();
        entryCancle.eventID = EventTriggerType.Cancel;
        entryCancle.callback.AddListener((eventData) => { AnimateTouchUp(); });
        eventTrigger.triggers.Add(entryCancle);
    }

    public void OverrideOriginalScale(Vector3 scale)
    {
        originalScale = scale;
    }

    public void AnimateTouchDown()
    {
        isTouchDownAnimationComplete = false;
        target.DOScale(originalScale * 0.95f, 0.1f)
            .SetEase(Ease.OutFlash)
            .SetUpdate(true)
            .OnComplete(() => isTouchDownAnimationComplete = true);
    }

    public void AnimateTouchUp()
    {
        if (isTouchDownAnimationComplete)
        {
            target.DOScale(originalScale * 1.05f, 0.1f)
                .SetEase(Ease.InOutQuad)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    target.DOScale(originalScale, 0.1f).SetEase(Ease.InOutQuad).SetUpdate(true);
                });
        }
        else
        {
            target.DOScale(originalScale * 0.95f, 0.1f)
                .SetEase(Ease.OutFlash)
                .SetUpdate(true)
                .OnComplete(() =>
                { 
                    isTouchDownAnimationComplete = true;
                    AnimateTouchUp();
                });
        }
    }
}
