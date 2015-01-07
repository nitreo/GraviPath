using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


public class MenuRootController : MenuRootControllerBase {
    
    public override void InitializeMenuRoot(MenuRootViewModel menuRoot) {
        Debug.Log("init");
    }


    public override void UpdateUniversesList(MenuRootViewModel menuRoot, UniverseListUpdateDescriptor arg)
    {
        base.UpdateUniversesList(menuRoot, arg);
        if (arg.Type == UniverseListUpdateType.Latest)
        {



            foreach (var item in menuRoot.UniversesList.ToList())
            {
                menuRoot.UniversesList.Remove(item);                                
            }
            UniverseRepository.GetLatestPaged(10, 0).Subscribe(universe =>
            {
                MenuRoot.UniversesList.Add(universe);
            });
        }
    }
}
