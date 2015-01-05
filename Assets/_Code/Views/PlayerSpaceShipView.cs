using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Thinksquirrel.Phys2D;
using UnityEngine;
using UniRx;


public partial class PlayerSpaceShipView
{ 



    /// Subscribes to the state machine property and executes a method for each state.
    public override void ShipStateChanged(Invert.StateMachine.State value) {
        base.ShipStateChanged(value);
    }
    
    public override void OnAlive() {
        base.OnAlive();

        NormalArt.SetActive(true);
        CrashedArt.SetActive(false);
        rigidbody2D.isKinematic = false;
          
    
    }
    
    public override void OnCrashed() {
        base.OnCrashed();

        NormalArt.SetActive(false);
        CrashedArt.SetActive(true);
        
        rigidbody2D.isKinematic = true;
        transform.position = LastCollisionPoint;

        Fire.SetActive(false);
        Swearing.SetActive(false);
        Boom.SetActive(true);

        Observable.Timer(TimeSpan.FromMilliseconds(300))
            .Subscribe(_ =>
            {
                Crashed.transform.up = -(transform.position - LastCollidedGravityObject.transform.position);
                Fire.SetActive(true);
                Swearing.SetActive(true);
                Boom.SetActive(false);
            }).DisposeWith(this);



    }

    protected override IObservable<Vector3> GetPositionObservable()
    {
        return PositionAsObservable;
    }


    public GameObject NormalArt;
    public GameObject CrashedArt;
    public GameObject Fire;
    public GameObject Swearing;
    public GameObject Boom;
    public GameObject Crashed;
    private ControllerFilter2DExt ignoreControllers;

    [HideInInspector]
    public GravityObject LastCollidedGravityObject { get; set; }
    [HideInInspector]
    public Vector3 LastCollisionPoint { get; set; }

    /// Invokes ResetExecuted when the Reset command is executed.
    public override void ResetExecuted() {
        base.ResetExecuted();
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.Sleep();
        transform.up = Vector3.up;
    }
 
    public override void AccelerateExecuted() {
        base.AccelerateExecuted();
        var force = Player.Direction;
        this.rigidbody2D.AddForce(new Vector2(force.x,force.y),ForceMode2D.Impulse);
    }
 
    public override void DirectionChanged(Vector3 value) {
        base.DirectionChanged(value);

        //transform.LookAt(transform.position + value.normalized);
        if(!(Player.ShipState is Crashed)) 
            transform.up = value;
    }

    public override void DockExecuted()
    {
        base.DockExecuted();
        var desc = Player.Dock.Parameter as DockDescriptor;
        ignoreControllers.IgnoreControllerType(ControllerType.GravityController);

        var dempedVelocity = new Vector3(rigidbody2D.velocity.x, rigidbody2D.velocity.y, 0);

        Observable.EveryUpdate()
            .TakeWhile(_ => Math.Abs(rigidbody2D.velocity.magnitude) > 0.05f)
            .Subscribe(_ =>
            {
                rigidbody2D.velocity = Vector3.Lerp(rigidbody2D.velocity, Vector2.zero, Time.deltaTime*4);
            }, _ =>
            {
                UnityEngine.Debug.Log("Stopped");
                rigidbody2D.Sleep();
            }).DisposeWhenChanged(Player.IsControllableProperty)
            .DisposeWith(this);

    }

    public override void Bind()
    {
        ignoreControllers = gameObject.AddComponent<ControllerFilter2DExt>();
        ignoreControllers.IgnoreControllerType(ControllerType.GravityController);

        base.Bind();
        ShipController.Commited += ExecuteAccelerate;

        Observable.EveryUpdate().Subscribe(_ =>
        {
            if (!Player.IsControllable)
            {
                //TODO: Fix the lagging on low velocity
                transform.up = rigidbody2D.velocity.normalized;
            }
        }).DisposeWith(this);

    }

    public override void IsControllableChanged(Boolean value) {
        base.IsControllableChanged(value);
        if (value)
        {
            ShipController
                .Direction
                .Subscribe(InputDirectionChanged)
                .DisposeWhenChanged(Player.IsControllableProperty);

            ShipController
                .Acceleration
                .Subscribe(InputAccelrationChanged)
                .DisposeWhenChanged(Player.IsControllableProperty);
         
            ignoreControllers.IgnoreControllerType(ControllerType.GravityController);
        }
        else
        {

            //Handle gravitational objects
            this.BindComponentCollision2DWith<GravityObject>(CollisionEventType.Enter, (obj, col) =>
            {
                LastCollidedGravityObject = obj;
                LastCollisionPoint = col.contacts.First().point;
                ExecuteCrash();
            }).DisposeWhenChanged(Player.IsControllableProperty);

            //Handle zones
            this.BindViewTrigger2DWith<ZoneView>(CollisionEventType.Enter, (obj) =>
            {
                ExecuteZoneReached(obj.Zone);
            }).DisposeWhenChanged(Player.IsControllableProperty); ;
            
            //Handle pickupables
            this.BindViewTrigger2DWith<PickupableView>(CollisionEventType.Enter, (obj) =>
            {
                ExecuteItemPickedUp(obj.Pickupable);
                obj.ExecutePickUp();
            }).DisposeWhenChanged(Player.IsControllableProperty); ;


            ignoreControllers.RestoreControllerType(ControllerType.GravityController);
            
        }
    }

    public void InputDirectionChanged(Vector3 dir)
    {
        this.ExecuteSetDirection(dir);
    }

    public void InputAccelrationChanged(float acc)
    {
        this.ExecuteSetAcceleration(acc);
    }
}
