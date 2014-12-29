using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;
using UniRx;


public partial class TryEntryView
{

    public GameObject Cross;


    private float summMagnitude;
    private Vector3 lastPosition;
    /// Subscribes to the property and is notified anytime the value changes.
    public override void TargetChanged(PlayerViewModel value) {
        base.TargetChanged(value);


        if (TryEntry.TargetProperty.LastValue != null && value == null)
        {
            var go = Instantiate(Cross) as GameObject;
            go.transform.SetParent(transform);
            go.transform.position = transform.position;

        }


        if(value != null) 
            value.PositionProperty.Subscribe(p =>
            {
                transform.position = new Vector3(p.x, p.y, -1);
                var magnitude = (transform.position - lastPosition).magnitude;
                summMagnitude += magnitude;    
                PathLength.OnNext(summMagnitude);
                lastPosition = transform.position;
            }).DisposeWhenChanged(TryEntry.TargetProperty)
            .DisposeWith(this)
            .DisposeWith(TryEntry);

    }

    public Subject<float> PathLength = new Subject<float>();


    protected override IObservable<float> GetPathLengthObservable()
    {
        return PathLength;
    }
}
