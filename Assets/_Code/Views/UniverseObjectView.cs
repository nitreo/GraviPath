using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Thinksquirrel.Phys2D;
using ThinksquirrelSoftware.Phys2D;
using UnityEngine;
using UniRx;
using UnityEditor;


public partial class UniverseObjectView
{


    private GameObject _handles;
    private GameObject _editor;

    public virtual GameObject GetEditorPrototype()
    {
        return Resources.Load<GameObject>("UniverseObjectEditorUI");
    }



    public void PopEffect()
    {
        transform.localScale = Vector3.zero;
        var t = LeanTween.scale(gameObject, UniverseObject.StartScale, 0.8f)
            .setEase(LeanTweenType.easeOutElastic);
        
        if (!UniverseObject.IsEditable)
        {
            var range = UnityEngine.Random.Range(0.1f, 0.5f);
            /* Observable.Timer(TimeSpan.FromMilliseconds((range) * 1000)).Subscribe(_ =>
            {
                GenericAudioSource.instance.PlayPop();
            });*/
            t.setDelay(range);
        }
    }

    #region Properties

    private GameObject Editor
    {
        get { return _editor ?? (_editor = GetEditorPrototype()); }
    }

    private GameObject Handles
    {
        get
        {
            if (_handles == null)
            {
                _handles = Instantiate(Editor) as GameObject;
                _handles.transform.SetParent(transform);
                _handles.GetComponent<UniverseObjectEditorUI>().Init();
            }
            return _handles;
        }
        set { _handles = value; }
    }

    #endregion

    #region Bindings
    public override void ResetExecuted() {
        base.ResetExecuted();
        transform.position = UniverseObject.StartPosition;
        transform.eulerAngles = UniverseObject.StartRotation;
        transform.localScale = UniverseObject.StartScale;
    }

    public override void IsEditableChanged(Boolean value)
    {
        base.IsEditableChanged(value);
        Handles.SetActive(value);
    }

    #endregion

    #region Observables

    protected override IObservable<Vector3> GetPositionObservable()
    {
        return PositionAsObservable;
    }

    protected override IObservable<Vector3> GetRotationObservable()
    {
        return RotationAsObservable.Select(q=>q.eulerAngles);
    }

    protected override IObservable<Vector3> GetScaleObservable()
    {
        return ScaleAsObservable;
    }

    #endregion
}

