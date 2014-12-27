using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;


public abstract partial class ShipController
{

    public abstract IObservable<Vector3> Direction { get; }
    public abstract IObservable<float> Acceleration{ get; }

    public abstract event Action Commited;

    public abstract event Action Canceled;

}
