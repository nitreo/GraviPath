using System.Collections.Generic;
using UniRx;
using UnityEngine;

public partial class TryEntryView
{
    public GameObject Cross;
    private Vector3 lastPosition;
    public Subject<float> PathLength = new Subject<float>();
    private float summMagnitude;

    /// Invokes ResetExecuted when the Reset command is executed.
    public override void ResetExecuted()
    {
        base.ResetExecuted();
    }

    /// Subscribes to the property and is notified anytime the value changes.
    public override void TargetChanged(PlayerViewModel value)
    {
        base.TargetChanged(value);

        if (TryEntry.TargetProperty.LastValue != null && value == null)
        {
            var go = Instantiate(Cross) as GameObject;
            go.transform.SetParent(transform);
            go.transform.position = transform.position;
        }

        if (value != null)
        {

            value.IsControllableProperty
                .Where(p => !p)
                .First()
                .Subscribe(_ =>
                {
                    lastPosition = transform.position;
                    summMagnitude = 0;
                })
                .DisposeWith(this)
                .DisposeWith(TryEntry);

            value.PositionProperty
                .Where(_ => !value.IsControllable)
                .Subscribe(p =>
                {
                    transform.position = new Vector3(p.x, p.y, -1);
                    var magnitude = (transform.position - lastPosition).magnitude;
                    summMagnitude += magnitude;
                    PathLength.OnNext(summMagnitude);
                    lastPosition = transform.position;
                })
                .DisposeWhenChanged(TryEntry.TargetProperty)
                .DisposeWith(this)
                .DisposeWith(TryEntry);
        }
    }

    protected override IObservable<float> GetPathLengthObservable()
    {
        return PathLength;
    }
}