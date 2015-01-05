using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;


public abstract partial class PickupableView {
    
    /// Subscribes to the property and is notified anytime the value changes.
    public override void IsActiveChanged(Boolean value) {
        base.IsActiveChanged(value);
        gameObject.SetActive(value);
    }
}
