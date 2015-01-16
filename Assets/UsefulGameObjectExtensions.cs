using UniRx;
using UnityEngine;

public static class UsefulGameObjectExtensions
{
    public static IObservable<Vector3> PositionAsObservable(this Transform transform)
    {
        return Observable.EveryUpdate()
            .Where(_ => transform.hasChanged)
            .Select(_ => transform.position);
    } 

    public static IObservable<Vector3> PositionAsObservable(this Transform transform, float thresold)
    {
        return transform.PositionAsObservable()
            .Scan(transform.position, (prev, cur) => (cur - prev).magnitude > thresold ? cur : prev)
            .DistinctUntilChanged();
    }

    public static IObservable<Vector3> Thresold(this IObservable<Vector3> original, float thresold)
    {
            return original
            .Scan((prev, cur) => (cur - prev).magnitude > thresold ? cur : prev)
            .DistinctUntilChanged();
        
    } 

    
}