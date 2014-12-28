using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TouchScript.Gestures;
using UnityEngine;
using UniRx;


public partial class InputShipController
{

    public PanGesture PanGesture;
    public Transform PivotPoint;
    public override event Action Commited;
    public override event Action Canceled;
    public double AccelerationThresold;
    public DirectionIndicator Indicator;

    private readonly Subject<Vector3> _directionObservable = new Subject<Vector3>();
    private readonly Subject<float> _accelarationObservable = new Subject<float>();    
    private Vector3 _pivotPosition = Vector3.zero;
    private Vector3 _pivotDelta = Vector3.zero;

    


    public override IObservable<Vector3> Direction
    {
        get { return _directionObservable; }
    }

    public override IObservable<float> Acceleration
    {
        get { return _accelarationObservable; }
    }

    public void Start()
    {
        PanGesture.PanStarted += (sender, args) =>
        {
            _pivotPosition = PivotPoint.position;
            _pivotDelta = _pivotPosition - transform.position;
            Indicator.gameObject.SetActive(true);
        };

        PanGesture.PanCompleted += (sender, args) =>
        {
            AccelerationThresold = 0.5;
            if (_pivotDelta.magnitude > AccelerationThresold && Commited != null) Commited();
            else if (Canceled != null) Canceled();
            Indicator.gameObject.SetActive(false);
        };

        PanGesture.Panned += (sender, args) =>
        {
            _pivotDelta -= PanGesture.LocalDeltaPosition;
            _directionObservable.OnNext(_pivotDelta);
            _accelarationObservable.OnNext(_pivotDelta.magnitude);
            Indicator.SetDirection(_pivotDelta);
        };
    }


}
