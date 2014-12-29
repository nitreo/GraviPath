using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UniRx;
using UnityEngine;


public class UniverseController : UniverseControllerBase {
    
    public override void InitializeUniverse(UniverseViewModel universe)
    {

        universe.Objects
            .Where(args => args.Action == NotifyCollectionChangedAction.Add)
            .Select(args => args.NewItems[0] as UniverseObjectViewModel)
            .Subscribe(item =>
            {
                universe.IsEditableProperty.Subscribe(v =>
                {
                    item.IsEditable = v;
                });
                item.IsEditable = universe.IsEditable;
            });

    }


    public override void Save(UniverseViewModel universe)
    {
        base.Save(universe);
     //   universe.SaveUniverse();
    }

    public override void Load(UniverseViewModel universe, string arg)
    {
        base.Load(universe, arg);
       // universe.LoadUniverse(arg);
    }
}
