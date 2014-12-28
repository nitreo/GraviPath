using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;


public partial class AnotherShipController {
    public override IObservable<Vector3> Direction
    {
        get { throw new NotImplementedException(); }
    }

    public override IObservable<float> Acceleration
    {
        get { throw new NotImplementedException(); }
    }

    public override event Action Commited;
    public override event Action Canceled;
}
