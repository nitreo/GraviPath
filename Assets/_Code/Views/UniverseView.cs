using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;


public partial class UniverseView { 

    /// Subscribes to the property and is notified anytime the value changes.
    public override void IsEditableChanged(Boolean value) {
        base.IsEditableChanged(value);
    }
 

    /// This binding will add or remove views based on an element/viewmodel collection.
    public override ViewBase CreateObjectsView(UniverseObjectViewModel item)
    {
        var objectsView = base.CreateObjectsView(item);

        objectsView.transform.position = item.Position;
        objectsView.transform.eulerAngles = item.Rotation;

        return objectsView;
    }

    /// This binding will add or remove views based on an element/viewmodel collection.
    public override void ObjectsAdded(ViewBase item) {
        base.ObjectsAdded(item);
        foreach (var hook in Hooks)
        {
            hook(item);
        }
    }
    
    /// This binding will add or remove views based on an element/vienLwmodel collection.
    public override void ObjectsRemoved(ViewBase item) {
        base.ObjectsRemoved(item);
        Destroy(item.gameObject);
    }

    public void AddUniverseObjectAddedHook(Action<ViewBase> hook)
    {
        Hooks.Add(hook);
    }

    public override void Bind()
    {
        base.Bind();
        if (Universe.Objects.Count > 0)
        {
            foreach (var uObject in Universe.Objects)
            {
                var view = CreateObjectsView(uObject);
                ObjectsAdded(view);
            }
        }
    }

    public HashSet<Action<ViewBase>> Hooks = new HashSet<Action<ViewBase>>();

}
