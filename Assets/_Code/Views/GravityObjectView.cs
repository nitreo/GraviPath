using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Thinksquirrel.Phys2D;
using UnityEngine;
using UniRx;


public partial class GravityObjectView {
    
    /// Invokes ResetExecuted when the Reset command is executed.
    public override void ResetExecuted() {
        base.ResetExecuted();
    }


    public override void IsEditableChanged(bool value)
    {
        base.IsEditableChanged(value);
        var colliderObject = GetComponent<PolygonCollider2D>();
        var gravityObject = GetComponent<GravityController2DExt>();
        if (value)
        {
            if(gravityObject)
            gravityObject.enabled = false;
            if(colliderObject)
            colliderObject.enabled = false;

        }
        else
        {
            if(gravityObject)
            gravityObject.enabled = true;
            if(colliderObject)
            colliderObject.enabled = true;
        }
    }


    public override GameObject GetEditorPrototype()
    {
        return Resources.Load<GameObject>("UniverseGravityObjectEditorUI");
    }

    public override void Read(ISerializerStream stream)
    {
    /*
        base.Read(stream);
        Debug.Log("Called");
        transform.position = stream.DeserializeVector3("Position");
        transform.rotation= stream.DeserializeQuaternion("Rotation");
    */
    }
}
