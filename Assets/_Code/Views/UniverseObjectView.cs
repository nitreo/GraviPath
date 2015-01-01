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

    public override void Awake()
    {
        base.Awake();
    }

    /// Subscribes to the property and is notified anytime the value changes.
    public override void IsEditableChanged(Boolean value)
    {
        base.IsEditableChanged(value);
        Handles.SetActive(value);
    }


    public virtual GameObject GetEditorPrototype()
    {
        return Resources.Load<GameObject>("UniverseObjectEditorUI");
    }

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
