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
        var objectsView = base.CreateObjectsView(item);
        var scaleCache = objectsView.transform.localScale;

        objectsView.transform.position = item.Position;
        objectsView.transform.eulerAngles = item.Rotation;
        objectsView.transform.localScale = Vector3.zero;


        //TODO: Move tweening and sound logic from here.
        var t = LeanTween.scale(objectsView.gameObject, scaleCache, 0.8f)
            .setEase(LeanTweenType.easeOutElastic);
        if (!Universe.IsEditable)
        {

            var range = UnityEngine.Random.Range(0.1f, 0.5f);

/*            Observable.Timer(TimeSpan.FromMilliseconds((range) * 1000)).Subscribe(_ =>
            {
                GenericAudioSource.instance.PlayPop();
            });*/

            t.setDelay(range);
        }

        //TODO: Move it somewhere else?
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
    }
    
    /// This binding will add or remove views based on an element/vienLwmodel collection.
    public override void ObjectsRemoved(ViewBase item) {
        base.ObjectsRemoved(item);
        Destroy(item.gameObject);
    }

    public override void Bind()
    {
        base.Bind();
    }

    

    public void AddGravityAffectedObject(Rigidbody2D body)
    {
        GravityObjects.ForEach(o=>o.AddRigidbody(body));
    }

}
