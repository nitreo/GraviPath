using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


public class PlayerController : PlayerControllerBase {
    
    public override void InitializePlayer(PlayerViewModel player) {
    }

    public override void SetAcceleration(PlayerViewModel player, float acceleration)
    {
        base.SetAcceleration(player, acceleration);
        player.Acceleration= acceleration;
    }

    public override void SetDirection(PlayerViewModel player, Vector3 direction)
    {
        base.SetDirection(player, direction);
        player.Direction = direction;
    }

    public override void Accelerate(PlayerViewModel player)
    {
        base.Accelerate(player);
        player.IsControllable = false;
    }

    public override void Reset(PlayerViewModel player)
    {
        base.Reset(player);
        player.IsControllable = true;
    }

    public override void ItemPickedUp(PlayerViewModel player, PickupableViewModel item)
    {
        base.ItemPickedUp(player, item);

        if (item is ScorePointViewModel)
        {
            player.ParentLevelRoot.BonusScore += 100;
        }
    }
}
