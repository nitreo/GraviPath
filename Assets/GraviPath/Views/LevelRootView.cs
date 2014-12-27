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

    public Text ScoreText;


    

    /// Subscribes to the property and is notified anytime the value changes.
    public override void ScoreChanged(Int32 value) {
        base.ScoreChanged(value);
        ScoreText.text = "Score: "+value;

    }
 

    /// Subscribes to the property and is notified anytime the value changes.
    public override void PlayerChanged(PlayerViewModel value) {
        base.PlayerChanged(value);
        if (value != null)
        {
            ExecuteRestart(false);
        }
    }
 

    /// This binding will add or remove views based on an element/viewmodel collection.
    public override ViewBase CreateAttemptsView(TryEntryViewModel item)
    {
        var go = InstantiateView(TryEntryPrefab, item);
            go.GetComponent<Trail>().LifeDecayEnabled(false);
        return go;
    }

    /// This binding will add or remove views based on an element/viewmodel collection.
    public override void AttemptsAdded(ViewBase item) {
        base.AttemptsAdded(item);
    }
    
    /// This binding will add or remove views based on an element/viewmodel collection.
    public override void AttemptsRemoved(ViewBase item) {
        base.AttemptsRemoved(item);
        Destroy(item.gameObject);
    }


    public GameObject TryEntryPrefab;

    /// Subscribes to the property and is notified anytime the value changes.
    public override void CurrentTryEntryChanged(TryEntryViewModel value) {

    }


    public Transform StartZone;

    public Button RestartButton;
    public Button QuitButton;

    /// Invokes RestartExecuted when the Restart command is executed.
    public override void RestartExecuted() {
        base.RestartExecuted();

        if (_Player != null)
        {
            _Player.transform.position = StartZone.position;
        }

        _smallAsteroidsPositions.Keys.ToList().ForEach(t =>
        {
            t.rigidbody2D.Sleep();
            t.position = _smallAsteroidsPositions[t];
            t.eulerAngles = _smallAsteroidsRotations[t];
        });

    }


    public override void Bind()
    {
        base.Bind();
        RestartButton.AsClickObservable()
            .Subscribe(_ =>{ExecuteRestart(true);})
            .DisposeWith(this); 
        
        QuitButton.AsClickObservable()
            .Subscribe(_ =>{ExecuteToMenu();})
            .DisposeWith(this);

        GameObject.FindGameObjectsWithTag("SmallAsteroid")
            .Select(o=>o.transform).ToList().ForEach(t =>
            {
                _smallAsteroidsPositions.Add(t,t.position);
                _smallAsteroidsRotations.Add(t,t.eulerAngles);
            });

    }

    private Dictionary<Transform, Vector3> _smallAsteroidsPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Vector3> _smallAsteroidsRotations = new Dictionary<Transform, Vector3>();

}
