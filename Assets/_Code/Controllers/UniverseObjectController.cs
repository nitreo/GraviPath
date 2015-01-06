using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


public class UniverseObjectController : UniverseObjectControllerBase {
    public override void InitializeUniverseObject(UniverseObjectViewModel universeObject) {
    
    }


    public override void Save(UniverseObjectViewModel universeObject)
    {
        base.Save(universeObject);
        universeObject.StartPosition = universeObject.Position;
        universeObject.StartRotation = universeObject.Rotation;
        universeObject.StartScale = universeObject.Scale;
    }
}
