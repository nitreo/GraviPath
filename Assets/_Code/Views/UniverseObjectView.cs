using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;


public partial class UniverseObjectView { 

    /// Invokes ResetExecuted when the Reset command is executed.
    public override void ResetExecuted() {
        base.ResetExecuted();
    }

    protected override IObservable<Vector3> GetPositionObservable()
    {
        return PositionAsObservable;
    }

    protected override IObservable<Vector3> GetRotationObservable()
    {
        return RotationAsObservable.Select(q=>q.eulerAngles);
    }
}
