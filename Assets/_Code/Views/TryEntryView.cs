using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniRx;
using UnityEngine;
using Vectrosity;

public partial class TryEntryView
{
    public GameObject Cross;
    private Vector3 lastPosition;
    public Subject<float> PathLength = new Subject<float>();
    private float summMagnitude;
    private VectorLine line;
    public Material lineMaterial;
    /// Invokes ResetExecuted when the Reset command is executed.
    public override void ResetExecuted()
    {
        base.ResetExecuted();
    }


    public override void Bind()
    {
        line = new VectorLine("TryEntry: "+Identifier,new List<Vector3>(),lineMaterial,10,LineType.Continuous);
        line.textureScale = 2.0f;
        line.continuousTexture = false;
        base.Bind();
    }

    /// Subscribes to the property and is notified anytime the value changes.
    public override void TargetChanged(PlayerViewModel value)
    {
        base.TargetChanged(value);

        if (TryEntry.TargetProperty.LastValue != null && value == null)
        {
            var go = Instantiate(Cross) as GameObject;
            go.transform.SetParent(transform);
            go.transform.position = lastPosition;
        }

        if (value != null)
        {

            
            value.Accelerate.Subscribe(_ =>
            {
                var thisCamera = Camera.main;
                VectorLine.SetCamera3D(thisCamera);
                lastPosition = value.Position;
                value.PositionProperty
                    .Thresold(0.2f)
                    .Subscribe(pos =>
                    {
                        line.points3.Add(pos);
                        summMagnitude += (pos - lastPosition).magnitude;
                        PathLength.OnNext(summMagnitude);
                        lastPosition = pos;
                    }).DisposeWith(this)
                    .DisposeWith(TryEntry)
                    .DisposeWhenChanged(TryEntry.TargetProperty);
                Observable.EveryUpdate().Subscribe(__ =>
                {
                    line.SetWidth(80 / Camera.main.orthographicSize);
                    line.Draw3D();
                }).DisposeWith(this)
                    .DisposeWith(TryEntry);

            }).DisposeWith(this)
              .DisposeWith(TryEntry)
              .DisposeWhenChanged(TryEntry.TargetProperty);

        }
    }

    public override void Unbind()
    {
        base.Unbind();
        VectorLine.Destroy(new []{line});
    }

    protected override IObservable<float> GetPathLengthObservable()
    {
        return PathLength;
    }
}