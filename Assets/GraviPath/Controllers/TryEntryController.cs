using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


public class TryEntryController : TryEntryControllerBase {
    
    public override void InitializeTryEntry(TryEntryViewModel tryEntry)
    {

        tryEntry.TargetProperty.Subscribe(t=>TargetChanged(tryEntry,t));

    }


    public void TargetChanged(TryEntryViewModel tryEntry,PlayerViewModel target)
    {

        if (target != null)
        {
            target.Reset.Subscribe(_ => { tryEntry.Target = null; });
        }
    }
}
