using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic;
using UnityEngine;
using UniRx;


public partial class BasicCameraController
{

    public Transform LocalTarget;
    public Vector3 GlobalPoint;
    public float GlobalSize;
    public bool ApplyVelocityFactor;
    public Vector2 Offset = new Vector2(0, 3);
    public float CameraSpeed = 4;
    public bool VelocityBasedSize;
    public float MinimumSize;
    public float SizeVelocityFactor;
    public float SizeChangeSpeed;
    private Rigidbody2D _velocityRigidbody2D;
    private Transform _cameraTransform;

    public override void Awake()
    {
        _velocityRigidbody2D = LocalTarget.GetComponent<Rigidbody2D>();
        if (_velocityRigidbody2D == null) ApplyVelocityFactor = false;

        _cameraTransform = Camera.transform;

        base.Awake();
    }

    void LocalUpdate()
    {
        if (!LocalTarget)
            return;

        Vector3 pos = _cameraTransform.position;
        Vector3 targetPos = LocalTarget.position;
        _cameraTransform.position = Vector3.Lerp(pos, new Vector3(targetPos.x + Offset.x, targetPos.y + Offset.y, pos.z), Time.deltaTime * CameraSpeed);

        if(ApplyVelocityFactor)
        Camera.orthographicSize = Mathf.Lerp(Camera.orthographicSize, MinimumSize + _velocityRigidbody2D.velocity.magnitude * SizeVelocityFactor, Time.deltaTime * SizeChangeSpeed);


    }

    public override void GlobalState()
    {
        base.GlobalState();

        var root = (View as LevelRootView).LevelRoot;
        
        var startZone = root.Universe.Objects.OfType<StartZoneViewModel>().FirstOrDefault();
        var winZone = root.Universe.Objects.OfType<WinZoneViewModel>().FirstOrDefault();
        if (startZone != null && winZone != null)
        {
            var sPos = startZone.Position;
            var wPos = winZone.Position;

            GlobalSize = Mathf.Max(Math.Abs(sPos.x - wPos.x), Math.Abs(sPos.y - wPos.y)) * 0.6f;

            GlobalPoint = sPos + (wPos - sPos) / 2;
        }

        Camera.orthographicSize = GlobalSize;
        _cameraTransform.SetXY(GlobalPoint.x,GlobalPoint.y);
    }

    public override void LocalState()
    {
        base.LocalState();

        Observable.EveryUpdate().Subscribe(_ =>
        {
            LocalUpdate();
        }).DisposeWhenChanged(StateProperty)
            .DisposeWith(View);

    }

    public override void StartState()
    {
        base.StartState();

    }
}
