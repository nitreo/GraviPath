using System;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UGUIExtensions
{

    

    public static IObservable<Unit> AsClickObservable(this Button button)
    {

        return Observable.Create<Unit>(observer =>
        {
            UnityAction unityAction = () => observer.OnNext(Unit.Default);
            button.onClick.AddListener(unityAction);
            return Disposable.Create(() => button.onClick.RemoveListener(unityAction));
        });
    }

    public static IObservable<string> AsSubmitObservable(this InputField inputField)
    {
        return Observable.Create<string>(observer =>
        {
            UnityAction<string> unityAction = observer.OnNext;
            inputField.onSubmit.AddListener(unityAction);
            return Disposable.Create(() => inputField.onSubmit.AddListener(unityAction));
        });
    }

    public static IObservable<float> AsValueChangedObservable(this Slider slider)
    {

        return Observable.Create<float>(observer =>
        {
            UnityAction<float> unityAction = observer.OnNext;
            slider.onValueChanged.AddListener(unityAction);
            return Disposable.Create(() => slider.onValueChanged.RemoveListener(unityAction));
        });
    }
    public static IObservable<bool> AsValueChangedObservable(this Toggle toggle)
    {

        return Observable.Create<bool>(observer =>
        {
            UnityAction<bool> unityAction = observer.OnNext;
            toggle.onValueChanged.AddListener(unityAction);
            return Disposable.Create(() => toggle.onValueChanged.RemoveListener(unityAction));
        });
    }
    
    public static IObservable<float> AsValueChangedObservable(this Scrollbar scrollbar)
    {

        return Observable.Create<float>(observer =>
        {
            UnityAction<float> unityAction = observer.OnNext;
            scrollbar.onValueChanged.AddListener(unityAction);
            return Disposable.Create(() => scrollbar.onValueChanged.RemoveListener(unityAction));
        });
    }
    public static IObservable<BaseEventData> AsObservableOfClick(this EventTrigger trigger)
    {
        return trigger.AsObservableOf(EventTriggerType.PointerClick);
    }
    public static IObservable<BaseEventData> AsObservableOfDrag(this EventTrigger trigger)
    {
        return trigger.AsObservableOf(EventTriggerType.Drag);
    }
    public static IObservable<BaseEventData> AsObservableOfMove(this EventTrigger trigger)
    {
        return trigger.AsObservableOf(EventTriggerType.Move);
    }
    public static IObservable<BaseEventData> AsObservableOfDrop(this EventTrigger trigger)
    {
        return trigger.AsObservableOf(EventTriggerType.Drop);
    }
    public static IObservable<BaseEventData> AsObservableOfPress(this EventTrigger trigger)
    {
        return trigger.AsObservableOf(EventTriggerType.PointerDown);
    }
    public static IObservable<BaseEventData> AsObservableOfRelease(this EventTrigger trigger)
    {
        return trigger.AsObservableOf(EventTriggerType.PointerUp);
    }
    public static IObservable<BaseEventData> AsObservableOfHover(this EventTrigger trigger)
    {
        return trigger.AsObservableOf(EventTriggerType.PointerEnter);
    }
    public static IObservable<BaseEventData> AsObservableOfExit(this EventTrigger trigger)
    {
        return trigger.AsObservableOf(EventTriggerType.PointerExit);
    }
    public static IObservable<BaseEventData> AsObservableOfScroll(this EventTrigger trigger)
    {
        return trigger.AsObservableOf(EventTriggerType.Scroll);
    }
    public static IObservable<BaseEventData> AsObservableOfSelect(this EventTrigger trigger)
    {
        return trigger.AsObservableOf(EventTriggerType.Select);
    }
    public static IObservable<BaseEventData> AsObservableOfUpdateSelected(this EventTrigger trigger)
    {
        return trigger.AsObservableOf(EventTriggerType.UpdateSelected);
    }  
    public static IObservable<BaseEventData> AsObservableOf(this EventTrigger trigger, EventTriggerType type)
    {
        return Observable.Create<BaseEventData>(observer =>
        {
            var entry = ComposeEntry(type, observer.OnNext);
            trigger.delegates.Add(entry);
            return Disposable.Create(() => trigger.delegates.Remove(entry));
        });
    }
    //Will the callback leak?
    private static EventTrigger.Entry ComposeEntry(EventTriggerType type, Action<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry
        {
            eventID = type,
            callback = new EventTrigger.TriggerEvent()
        };
        var callback = new UnityAction<BaseEventData>(action);
        
        entry.callback.AddListener(callback);
        return entry;
    }

}