using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UnityEditor;


public partial class UniverseObjectView
{

    private GameObject editor;
    private GameObject handles;
    public override void Awake()
    {
        editor = Resources.Load<GameObject>("UniverseObjectEditorUI");
        base.Awake();
    }

    /// Subscribes to the property and is notified anytime the value changes.
    public override void IsEditableChanged(Boolean value) {
        base.IsEditableChanged(value);

        if (value)
        {
            if (handles == null)
            {
                handles = Instantiate(editor) as GameObject;
                handles.transform.SetParent(this.transform);
                handles.GetComponent<UniverseObjectEditorUI>().Init();
            }
            else
            {
                handles.SetActive(true);
            }
        }
        else
        {
            if(handles!=null)
            handles.SetActive(false);
        }
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
