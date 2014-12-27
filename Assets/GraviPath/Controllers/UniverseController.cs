using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


public class UniverseController : UniverseControllerBase {
    
    public override void InitializeUniverse(UniverseViewModel universe) {
    }


    public override void Save(UniverseViewModel universe)
    {
        base.Save(universe);
        universe.SaveUniverse();
    }

    public override void Load(UniverseViewModel universe, string arg)
    {
        base.Load(universe, arg);
        universe.LoadUniverse(arg);
    }
}
