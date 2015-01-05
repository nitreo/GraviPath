using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


public class PickupableController : PickupableControllerBase {
    
    public override void InitializePickupable(PickupableViewModel pickupable)
    {
        pickupable.IsActive = true;
    }


    public override void PickUp(PickupableViewModel pickupable)
    {
        base.PickUp(pickupable);
        pickupable.IsActive = false;
    }

    public override void Reset(UniverseObjectViewModel universeObject)
    {
        base.Reset(universeObject);
        var pickupable = universeObject as PickupableViewModel;
        if (pickupable != null) pickupable.IsActive = true;
        else throw new Exception("Lol");
    }
}
