using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;


public partial class CameraController
{

    public Camera Camera;
    public CameraControlState InitialState;        
    protected readonly P<CameraControlState> StateProperty = new P<CameraControlState>();

    private CameraControlState State
    {
        get { return StateProperty.Value; }
        set { StateProperty.Value = value; }
    }


    public override void Bind(ViewBase view)
    {
        base.Bind(view);

        StateProperty.Subscribe(state =>
        {
            switch (state)
            {
                case CameraControlState.Global:
                    GlobalState();
                    break;
                case CameraControlState.Local:
                    LocalState();
                    break;
                case CameraControlState.Start:
                    StartState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("state");
            }
        });

        State = InitialState;

        Observable
            .EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Tab))
            .Subscribe(_ =>
            {
                ToggleState();
            })
            .DisposeWith(view)
            .DisposeWith(gameObject);

        

    }
    private void ToggleState()
    {
        switch (State)
        {
            case CameraControlState.Global:
                State = CameraControlState.Local;
                break;
            case CameraControlState.Local:
                State = CameraControlState.Start;
                break;
            case CameraControlState.Start:
                State = CameraControlState.Global;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    public virtual void GlobalState()
    {
        
    }
    public virtual void LocalState()
    {
        
    }
    public virtual void StartState()
    {
        
    }



}

public enum CameraControlState
{
    Global,
    Local,
    Start
}

