using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Thinksquirrel.Phys2D;
using UnityEngine;
using UniRx;


public partial class UniverseView { 

    /// Subscribes to the property and is notified anytime the value changes.
    public override void IsEditableChanged(Boolean value) {
        base.IsEditableChanged(value);
    }

    public HashSet<Action<ViewBase>> Hooks = new HashSet<Action<ViewBase>>();
    private readonly HashSet<GravityController2DExt> GravityObjects = new HashSet<GravityController2DExt>();

    /// This binding will add or remove views based on an element/viewmodel collection.
    public override ViewBase CreateObjectsView(UniverseObjectViewModel item)
    {
        var objectsView = base.CreateObjectsView(item) as UniverseObjectView;


        //Scale fix, against the objects of the first generation
        if (item.StartScale == Vector3.zero) item.StartScale = objectsView.transform.localScale;

        //Reset bitch everything. (transition from prefab state to saved entity state)
        objectsView.ExecuteCommand(item.Reset);

        objectsView.PopEffect();

        return objectsView;
    }

    public override void ObjectsAdded(ViewBase item) {
        base.ObjectsAdded(item);
        if (item.ViewModelObject is GravityObjectViewModel)
        {
            var gravController = item.GetComponent<GravityController2DExt>();
            if (gravController != null) GravityObjects.Add(gravController);
        }
    }
    
    public override void ObjectsRemoved(ViewBase item) {
        base.ObjectsRemoved(item);
        Destroy(item.gameObject);
    }

    public void AddGravityAffectedObject(Rigidbody2D body)
    {
        GravityObjects.ForEach(o=>o.AddRigidbody(body));
    }

}
