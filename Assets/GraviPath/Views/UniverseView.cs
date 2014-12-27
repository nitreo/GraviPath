using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;


public partial class UniverseView { 

    /// This binding will add or remove views based on an element/viewmodel collection.
    public override ViewBase CreateObjectsView(UniverseObjectViewModel item) {
        return base.CreateObjectsView(item);
    }
    
    /// This binding will add or remove views based on an element/viewmodel collection.
    public override void ObjectsAdded(ViewBase item) {
        base.ObjectsAdded(item);
    }
    
    /// This binding will add or remove views based on an element/viewmodel collection.
    public override void ObjectsRemoved(ViewBase item) {
        base.ObjectsRemoved(item);
    }

}
