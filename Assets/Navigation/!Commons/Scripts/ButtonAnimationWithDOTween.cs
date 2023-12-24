#region

using DG.Tweening;
using UnityEngine;

#endregion

public class ButtonAnimationWithDOTween : MonoBehaviour
{
    // EventTrigger eventTrigger;

    private void Awake()
    {
        // eventTrigger = GetComponent<EventTrigger>();
    }

    private void OnEnable()
    {
        // eventTrigger.triggers.Clear();
        // EventTrigger.Entry entry = new EventTrigger.Entry();
        // entry.eventID = EventTriggerType.PointerEnter;
        // entry.callback.AddListener((data) => { OnPointerEnter(); });
        // eventTrigger.triggers.Add(entry);
        //
        // entry = new EventTrigger.Entry();
        // entry.eventID = EventTriggerType.PointerExit;
        // entry.callback.AddListener((data) => { OnPointerExit(); });
        // eventTrigger.triggers.Add(entry);
    }


    public void OnPointerEnter()
    {
        transform.localScale = Vector3.one;
        transform.DOScale(Vector3.one * 2f, 0.25f).From().SetEase(Ease.OutBack);
    }

    public void OnPointerExit()
    {
        transform.DOScale(Vector3.one * 2f, 0.25f);
    }
}