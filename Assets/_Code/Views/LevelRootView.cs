using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;
using UniRx;
using UnityEngine.UI;


public partial class LevelRootView
{
 
    private UniverseView _currentUniverseView;
    public GameObject TryEntryPrefab;
    public UniverseView CreateUniverseView(UniverseViewModel viewModel)
    {
        var uni = InstantiateView(viewModel) as UniverseView;
        uni.AddGravityAffectedObject(_Player.rigidbody2D);
        return uni;
    }

    public override void UniverseChanged(UniverseViewModel value) {
        base.UniverseChanged(value);

        if (_currentUniverseView != null) Destroy(_currentUniverseView.gameObject);

        if (value != null)
        {
           _currentUniverseView = CreateUniverseView(value);
        }
    }

    public override ViewBase CreateAttemptsView(TryEntryViewModel item)
    {
        var go = InstantiateView(TryEntryPrefab, item);
        go.GetComponent<Trail>().LifeDecayEnabled(false);
        return go;
    }

    public override void AttemptsAdded(ViewBase item)
    {
        base.AttemptsAdded(item);
    }

    public override void AttemptsRemoved(ViewBase item)
    {
        base.AttemptsRemoved(item);
        Destroy(item.gameObject);
    }
 
    public override void PlayerChanged(PlayerViewModel value) {
        base.PlayerChanged(value);
        if (value != null) ExecuteRestart(false);
    }
 
    public override void CurrentTryEntryChanged(TryEntryViewModel value) {

    }

    public override void RestartExecuted() {
        base.RestartExecuted();
        if (_Player != null && LevelRoot.Universe !=null)
        {
            var zonePos = LevelRoot.Universe.Objects.OfType<StartZoneViewModel>().First().Position;
            _Player.transform.position = zonePos;
        }
    }

    public override void Bind()
    {
        base.Bind();

    }

}
