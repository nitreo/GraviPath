using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;


public partial class GravityObjectView {
    
    /// Invokes ResetExecuted when the Reset command is executed.
    public override void ResetExecuted() {
        base.ResetExecuted();
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
