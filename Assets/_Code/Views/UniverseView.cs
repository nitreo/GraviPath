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
 

    /// This binding will add or remove views based on an element/viewmodel collection.
    public override ViewBase CreateObjectsView(UniverseObjectViewModel item)
    {
        var objectsView = base.CreateObjectsView(item);

        objectsView.transform.position = item.Position;
        objectsView.transform.eulerAngles = item.Rotation;
        var cache = objectsView.transform.localScale;
        objectsView.transform.localScale = Vector3.zero;


        
        var t = LeanTween.scale(objectsView.gameObject, cache, 0.8f)
            .setEase(LeanTweenType.easeOutElastic);

        if (!Universe.IsEditable)
        {

            var range = UnityEngine.Random.Range(0.1f, 0.5f);

            Observable.Timer(TimeSpan.FromMilliseconds((range) * 1000)).Subscribe(_ =>
            {
                GenericAudioSource.instance.PlayPop();
            });

            t.setDelay(range);
        }


        if (item is GravityObjectViewModel)
        {
            var gravController = objectsView.GetComponent<GravityController2DExt>();
            if (gravController != null) GravityObjects.Add(gravController);
        }

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

    public void AddGravityAffectedObject(Rigidbody2D body)
    {
        GravityObjects.ForEach(o=>o.AddRigidbody(body));
    }

    public HashSet<Action<ViewBase>> Hooks = new HashSet<Action<ViewBase>>();
    private readonly HashSet<GravityController2DExt> GravityObjects = new HashSet<GravityController2DExt>();
}
