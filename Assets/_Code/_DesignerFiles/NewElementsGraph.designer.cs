using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[DiagramInfoAttribute("GraviPath")]
public class MenuRootViewModelBase : ViewModel {
    
    public ModelCollection<UniverseViewModel> _UniversesListProperty;
    
    protected CommandWithSenderAndArgument<MenuRootViewModel, StartLevelDescriptor> _StartLevel;
    
    protected CommandWithSender<MenuRootViewModel> _StartEditor;
    
    protected CommandWithSenderAndArgument<MenuRootViewModel, UniverseListUpdateDescriptor> _UpdateUniversesList;
    
    public MenuRootViewModelBase(MenuRootControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public MenuRootViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
        _UniversesListProperty = new ModelCollection<UniverseViewModel>(this, "UniversesList");
        _UniversesListProperty.CollectionChanged += UniversesListCollectionChanged;
    }
    
    protected virtual void UniversesListCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs args) {
    }
}

public partial class MenuRootViewModel : MenuRootViewModelBase {
    
    public MenuRootViewModel(MenuRootControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public MenuRootViewModel() : 
            base() {
    }
    
    public virtual ModelCollection<UniverseViewModel> UniversesList {
        get {
            return this._UniversesListProperty;
        }
    }
    
    public virtual CommandWithSenderAndArgument<MenuRootViewModel, StartLevelDescriptor> StartLevel {
        get {
            return _StartLevel;
        }
        set {
            _StartLevel = value;
        }
    }
    
    public virtual CommandWithSender<MenuRootViewModel> StartEditor {
        get {
            return _StartEditor;
        }
        set {
            _StartEditor = value;
        }
    }
    
    public virtual CommandWithSenderAndArgument<MenuRootViewModel, UniverseListUpdateDescriptor> UpdateUniversesList {
        get {
            return _UpdateUniversesList;
        }
        set {
            _UpdateUniversesList = value;
        }
    }
    
    protected override void WireCommands(Controller controller) {
        var menuRoot = controller as MenuRootControllerBase;
        this.StartLevel = new CommandWithSenderAndArgument<MenuRootViewModel, StartLevelDescriptor>(this, menuRoot.StartLevel);
        this.StartEditor = new CommandWithSender<MenuRootViewModel>(this, menuRoot.StartEditor);
        this.UpdateUniversesList = new CommandWithSenderAndArgument<MenuRootViewModel, UniverseListUpdateDescriptor>(this, menuRoot.UpdateUniversesList);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
        if (stream.DeepSerialize) stream.SerializeArray("UniversesList", this.UniversesList);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
if (stream.DeepSerialize) {
        this.UniversesList.Clear();
        this.UniversesList.AddRange(stream.DeserializeObjectArray<UniverseViewModel>("UniversesList"));
}
    }
    
    public override void Unbind() {
        base.Unbind();
        _UniversesListProperty.CollectionChanged -= UniversesListCollectionChanged;
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
        list.Add(new ViewModelPropertyInfo(_UniversesListProperty, true, true, false));
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
        list.Add(new ViewModelCommandInfo("StartLevel", StartLevel) { ParameterType = typeof(StartLevelDescriptor) });
        list.Add(new ViewModelCommandInfo("StartEditor", StartEditor) { ParameterType = typeof(void) });
        list.Add(new ViewModelCommandInfo("UpdateUniversesList", UpdateUniversesList) { ParameterType = typeof(UniverseListUpdateDescriptor) });
    }
    
    protected override void UniversesListCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs args) {
        foreach (var item in args.NewItems.OfType<UniverseViewModel>()) item.ParentMenuRoot = this;;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class LevelRootViewModelBase : ViewModel {
    
    private IDisposable _ScoreDisposable;
    
    public P<PlayerViewModel> _PlayerProperty;
    
    public P<TryEntryViewModel> _CurrentTryEntryProperty;
    
    public P<Int32> _BonusScoreProperty;
    
    public P<UniverseViewModel> _UniverseProperty;
    
    public P<Boolean> _RecordAttemptsProperty;
    
    public P<Int32> _ScoreProperty;
    
    public ModelCollection<TryEntryViewModel> _AttemptsProperty;
    
    protected CommandWithSender<LevelRootViewModel> _ToMenu;
    
    protected CommandWithSenderAndArgument<LevelRootViewModel, Boolean> _Restart;
    
    protected CommandWithSenderAndArgument<LevelRootViewModel, UniverseViewModel> _LoadUniverse;
    
    public LevelRootViewModelBase(LevelRootControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public LevelRootViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
        _PlayerProperty = new P<PlayerViewModel>(this, "Player");
        _CurrentTryEntryProperty = new P<TryEntryViewModel>(this, "CurrentTryEntry");
        _BonusScoreProperty = new P<Int32>(this, "BonusScore");
        _UniverseProperty = new P<UniverseViewModel>(this, "Universe");
        _RecordAttemptsProperty = new P<Boolean>(this, "RecordAttempts");
        _ScoreProperty = new P<Int32>(this, "Score");
        _AttemptsProperty = new ModelCollection<TryEntryViewModel>(this, "Attempts");
        _AttemptsProperty.CollectionChanged += AttemptsCollectionChanged;
        this.ResetScore();
        this.BindProperty(_CurrentTryEntryProperty, p=> ResetScore());
    }
    
    public virtual void ResetScore() {
        if (_ScoreDisposable != null) _ScoreDisposable.Dispose();
        _ScoreDisposable = _ScoreProperty.ToComputed( ComputeScore, this.GetScoreDependents().ToArray() ).DisposeWith(this);
    }
    
    protected virtual void AttemptsCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs args) {
    }
    
    public virtual Int32 ComputeScore() {
        return default(Int32);
    }
    
    public virtual IEnumerable<IObservableProperty> GetScoreDependents() {
        if (_CurrentTryEntryProperty.Value != null) {
            yield return _CurrentTryEntryProperty.Value._PathLengthProperty;
        }
        yield return _BonusScoreProperty;
        yield break;
    }
}

public partial class LevelRootViewModel : LevelRootViewModelBase {
    
    public LevelRootViewModel(LevelRootControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public LevelRootViewModel() : 
            base() {
    }
    
    public virtual P<PlayerViewModel> PlayerProperty {
        get {
            return this._PlayerProperty;
        }
    }
    
    public virtual PlayerViewModel Player {
        get {
            return _PlayerProperty.Value;
        }
        set {
            _PlayerProperty.Value = value;
            if (value != null) value.ParentLevelRoot = this;
        }
    }
    
    public virtual P<TryEntryViewModel> CurrentTryEntryProperty {
        get {
            return this._CurrentTryEntryProperty;
        }
    }
    
    public virtual TryEntryViewModel CurrentTryEntry {
        get {
            return _CurrentTryEntryProperty.Value;
        }
        set {
            _CurrentTryEntryProperty.Value = value;
            if (value != null) value.ParentLevelRoot = this;
        }
    }
    
    public virtual P<Int32> BonusScoreProperty {
        get {
            return this._BonusScoreProperty;
        }
    }
    
    public virtual Int32 BonusScore {
        get {
            return _BonusScoreProperty.Value;
        }
        set {
            _BonusScoreProperty.Value = value;
        }
    }
    
    public virtual P<UniverseViewModel> UniverseProperty {
        get {
            return this._UniverseProperty;
        }
    }
    
    public virtual UniverseViewModel Universe {
        get {
            return _UniverseProperty.Value;
        }
        set {
            _UniverseProperty.Value = value;
            if (value != null) value.ParentLevelRoot = this;
        }
    }
    
    public virtual P<Boolean> RecordAttemptsProperty {
        get {
            return this._RecordAttemptsProperty;
        }
    }
    
    public virtual Boolean RecordAttempts {
        get {
            return _RecordAttemptsProperty.Value;
        }
        set {
            _RecordAttemptsProperty.Value = value;
        }
    }
    
    public virtual P<Int32> ScoreProperty {
        get {
            return this._ScoreProperty;
        }
    }
    
    public virtual Int32 Score {
        get {
            return _ScoreProperty.Value;
        }
        set {
            _ScoreProperty.Value = value;
        }
    }
    
    public virtual ModelCollection<TryEntryViewModel> Attempts {
        get {
            return this._AttemptsProperty;
        }
    }
    
    public virtual CommandWithSender<LevelRootViewModel> ToMenu {
        get {
            return _ToMenu;
        }
        set {
            _ToMenu = value;
        }
    }
    
    public virtual CommandWithSenderAndArgument<LevelRootViewModel, Boolean> Restart {
        get {
            return _Restart;
        }
        set {
            _Restart = value;
        }
    }
    
    public virtual CommandWithSenderAndArgument<LevelRootViewModel, UniverseViewModel> LoadUniverse {
        get {
            return _LoadUniverse;
        }
        set {
            _LoadUniverse = value;
        }
    }
    
    protected override void WireCommands(Controller controller) {
        var levelRoot = controller as LevelRootControllerBase;
        this.ToMenu = new CommandWithSender<LevelRootViewModel>(this, levelRoot.ToMenu);
        this.Restart = new CommandWithSenderAndArgument<LevelRootViewModel, Boolean>(this, levelRoot.Restart);
        this.LoadUniverse = new CommandWithSenderAndArgument<LevelRootViewModel, UniverseViewModel>(this, levelRoot.LoadUniverse);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
		if (stream.DeepSerialize) stream.SerializeObject("Player", this.Player);
		if (stream.DeepSerialize) stream.SerializeObject("CurrentTryEntry", this.CurrentTryEntry);
        stream.SerializeInt("BonusScore", this.BonusScore);
		if (stream.DeepSerialize) stream.SerializeObject("Universe", this.Universe);
        stream.SerializeBool("RecordAttempts", this.RecordAttempts);
        if (stream.DeepSerialize) stream.SerializeArray("Attempts", this.Attempts);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
		if (stream.DeepSerialize) this.Player = stream.DeserializeObject<PlayerViewModel>("Player");
		if (stream.DeepSerialize) this.CurrentTryEntry = stream.DeserializeObject<TryEntryViewModel>("CurrentTryEntry");
        		this.BonusScore = stream.DeserializeInt("BonusScore");;
		if (stream.DeepSerialize) this.Universe = stream.DeserializeObject<UniverseViewModel>("Universe");
        		this.RecordAttempts = stream.DeserializeBool("RecordAttempts");;
if (stream.DeepSerialize) {
        this.Attempts.Clear();
        this.Attempts.AddRange(stream.DeserializeObjectArray<TryEntryViewModel>("Attempts"));
}
    }
    
    public override void Unbind() {
        base.Unbind();
        _AttemptsProperty.CollectionChanged -= AttemptsCollectionChanged;
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
        list.Add(new ViewModelPropertyInfo(_PlayerProperty, true, false, false));
        list.Add(new ViewModelPropertyInfo(_CurrentTryEntryProperty, true, false, false));
        list.Add(new ViewModelPropertyInfo(_BonusScoreProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_UniverseProperty, true, false, false));
        list.Add(new ViewModelPropertyInfo(_RecordAttemptsProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_ScoreProperty, false, false, false, true));
        list.Add(new ViewModelPropertyInfo(_AttemptsProperty, true, true, false));
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
        list.Add(new ViewModelCommandInfo("ToMenu", ToMenu) { ParameterType = typeof(void) });
        list.Add(new ViewModelCommandInfo("Restart", Restart) { ParameterType = typeof(Boolean) });
        list.Add(new ViewModelCommandInfo("LoadUniverse", LoadUniverse) { ParameterType = typeof(UniverseViewModel) });
    }
    
    protected override void AttemptsCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs args) {
        foreach (var item in args.NewItems.OfType<TryEntryViewModel>()) item.ParentLevelRoot = this;;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class PlayerViewModelBase : ViewModel {
    
    public P<Vector3> _DirectionProperty;
    
    public P<Single> _AccelerationProperty;
    
    public P<Boolean> _IsControllableProperty;
    
    public ShipStateMachine _ShipStateProperty;
    
    public P<Vector3> _PositionProperty;
    
    protected CommandWithSender<PlayerViewModel> _Accelerate;
    
    protected CommandWithSenderAndArgument<PlayerViewModel, Single> _SetAcceleration;
    
    protected CommandWithSenderAndArgument<PlayerViewModel, Vector3> _SetDirection;
    
    protected CommandWithSender<PlayerViewModel> _Reset;
    
    protected CommandWithSender<PlayerViewModel> _Crash;
    
    protected CommandWithSenderAndArgument<PlayerViewModel, ZoneViewModel> _ZoneReached;
    
    protected CommandWithSenderAndArgument<PlayerViewModel, DockDescriptor> _Dock;
    
    protected CommandWithSenderAndArgument<PlayerViewModel, PickupableViewModel> _ItemPickedUp;
    
    public PlayerViewModelBase(PlayerControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public PlayerViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
        _DirectionProperty = new P<Vector3>(this, "Direction");
        _AccelerationProperty = new P<Single>(this, "Acceleration");
        _IsControllableProperty = new P<Boolean>(this, "IsControllable");
        _ShipStateProperty = new ShipStateMachine(this, "ShipState");
        _PositionProperty = new P<Vector3>(this, "Position");
        this._Crash.Subscribe(_ShipStateProperty.Crash);
        this._Reset.Subscribe(_ShipStateProperty.Reset);
    }
}

public partial class PlayerViewModel : PlayerViewModelBase {
    
    private LevelRootViewModel _ParentLevelRoot;
    
    private TryEntryViewModel _ParentTryEntry;
    
    public PlayerViewModel(PlayerControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public PlayerViewModel() : 
            base() {
    }
    
    public virtual P<Vector3> DirectionProperty {
        get {
            return this._DirectionProperty;
        }
    }
    
    public virtual Vector3 Direction {
        get {
            return _DirectionProperty.Value;
        }
        set {
            _DirectionProperty.Value = value;
        }
    }
    
    public virtual P<Single> AccelerationProperty {
        get {
            return this._AccelerationProperty;
        }
    }
    
    public virtual Single Acceleration {
        get {
            return _AccelerationProperty.Value;
        }
        set {
            _AccelerationProperty.Value = value;
        }
    }
    
    public virtual P<Boolean> IsControllableProperty {
        get {
            return this._IsControllableProperty;
        }
    }
    
    public virtual Boolean IsControllable {
        get {
            return _IsControllableProperty.Value;
        }
        set {
            _IsControllableProperty.Value = value;
        }
    }
    
    public virtual ShipStateMachine ShipStateProperty {
        get {
            return this._ShipStateProperty;
        }
    }
    
    public virtual Invert.StateMachine.State ShipState {
        get {
            return _ShipStateProperty.Value;
        }
        set {
            _ShipStateProperty.Value = value;
        }
    }
    
    public virtual P<Vector3> PositionProperty {
        get {
            return this._PositionProperty;
        }
    }
    
    public virtual Vector3 Position {
        get {
            return _PositionProperty.Value;
        }
        set {
            _PositionProperty.Value = value;
        }
    }
    
    public virtual CommandWithSender<PlayerViewModel> Accelerate {
        get {
            return _Accelerate;
        }
        set {
            _Accelerate = value;
        }
    }
    
    public virtual CommandWithSenderAndArgument<PlayerViewModel, Single> SetAcceleration {
        get {
            return _SetAcceleration;
        }
        set {
            _SetAcceleration = value;
        }
    }
    
    public virtual CommandWithSenderAndArgument<PlayerViewModel, Vector3> SetDirection {
        get {
            return _SetDirection;
        }
        set {
            _SetDirection = value;
        }
    }
    
    public virtual CommandWithSender<PlayerViewModel> Reset {
        get {
            return _Reset;
        }
        set {
            _Reset = value;
        }
    }
    
    public virtual CommandWithSender<PlayerViewModel> Crash {
        get {
            return _Crash;
        }
        set {
            _Crash = value;
        }
    }
    
    public virtual CommandWithSenderAndArgument<PlayerViewModel, ZoneViewModel> ZoneReached {
        get {
            return _ZoneReached;
        }
        set {
            _ZoneReached = value;
        }
    }
    
    public virtual CommandWithSenderAndArgument<PlayerViewModel, DockDescriptor> Dock {
        get {
            return _Dock;
        }
        set {
            _Dock = value;
        }
    }
    
    public virtual CommandWithSenderAndArgument<PlayerViewModel, PickupableViewModel> ItemPickedUp {
        get {
            return _ItemPickedUp;
        }
        set {
            _ItemPickedUp = value;
        }
    }
    
    public virtual LevelRootViewModel ParentLevelRoot {
        get {
            return this._ParentLevelRoot;
        }
        set {
            _ParentLevelRoot = value;
        }
    }
    
    public virtual TryEntryViewModel ParentTryEntry {
        get {
            return this._ParentTryEntry;
        }
        set {
            _ParentTryEntry = value;
        }
    }
    
    protected override void WireCommands(Controller controller) {
        var player = controller as PlayerControllerBase;
        this.Accelerate = new CommandWithSender<PlayerViewModel>(this, player.Accelerate);
        this.SetAcceleration = new CommandWithSenderAndArgument<PlayerViewModel, Single>(this, player.SetAcceleration);
        this.SetDirection = new CommandWithSenderAndArgument<PlayerViewModel, Vector3>(this, player.SetDirection);
        this.Reset = new CommandWithSender<PlayerViewModel>(this, player.Reset);
        this.Crash = new CommandWithSender<PlayerViewModel>(this, player.Crash);
        this.ZoneReached = new CommandWithSenderAndArgument<PlayerViewModel, ZoneViewModel>(this, player.ZoneReached);
        this.Dock = new CommandWithSenderAndArgument<PlayerViewModel, DockDescriptor>(this, player.Dock);
        this.ItemPickedUp = new CommandWithSenderAndArgument<PlayerViewModel, PickupableViewModel>(this, player.ItemPickedUp);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
        stream.SerializeVector3("Direction", this.Direction);
        stream.SerializeFloat("Acceleration", this.Acceleration);
        stream.SerializeBool("IsControllable", this.IsControllable);
        stream.SerializeString("ShipState", this.ShipState.Name);;
        stream.SerializeVector3("Position", this.Position);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
        		this.Direction = stream.DeserializeVector3("Direction");;
        		this.Acceleration = stream.DeserializeFloat("Acceleration");;
        		this.IsControllable = stream.DeserializeBool("IsControllable");;
        this._ShipStateProperty.SetState(stream.DeserializeString("ShipState"));
        		this.Position = stream.DeserializeVector3("Position");;
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
        list.Add(new ViewModelPropertyInfo(_DirectionProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_AccelerationProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_IsControllableProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_ShipStateProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_PositionProperty, false, false, false));
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
        list.Add(new ViewModelCommandInfo("Accelerate", Accelerate) { ParameterType = typeof(void) });
        list.Add(new ViewModelCommandInfo("SetAcceleration", SetAcceleration) { ParameterType = typeof(Single) });
        list.Add(new ViewModelCommandInfo("SetDirection", SetDirection) { ParameterType = typeof(Vector3) });
        list.Add(new ViewModelCommandInfo("Reset", Reset) { ParameterType = typeof(void) });
        list.Add(new ViewModelCommandInfo("Crash", Crash) { ParameterType = typeof(void) });
        list.Add(new ViewModelCommandInfo("ZoneReached", ZoneReached) { ParameterType = typeof(ZoneViewModel) });
        list.Add(new ViewModelCommandInfo("Dock", Dock) { ParameterType = typeof(DockDescriptor) });
        list.Add(new ViewModelCommandInfo("ItemPickedUp", ItemPickedUp) { ParameterType = typeof(PickupableViewModel) });
    }
}

[DiagramInfoAttribute("GraviPath")]
public class TryEntryViewModelBase : ViewModel {
    
    public P<Int32> _NumberProperty;
    
    public P<PlayerViewModel> _TargetProperty;
    
    public P<Single> _PathLengthProperty;
    
    protected CommandWithSender<TryEntryViewModel> _Reset;
    
    public TryEntryViewModelBase(TryEntryControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public TryEntryViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
        _NumberProperty = new P<Int32>(this, "Number");
        _TargetProperty = new P<PlayerViewModel>(this, "Target");
        _PathLengthProperty = new P<Single>(this, "PathLength");
    }
}

public partial class TryEntryViewModel : TryEntryViewModelBase {
    
    private LevelRootViewModel _ParentLevelRoot;
    
    public TryEntryViewModel(TryEntryControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public TryEntryViewModel() : 
            base() {
    }
    
    public virtual P<Int32> NumberProperty {
        get {
            return this._NumberProperty;
        }
    }
    
    public virtual Int32 Number {
        get {
            return _NumberProperty.Value;
        }
        set {
            _NumberProperty.Value = value;
        }
    }
    
    public virtual P<PlayerViewModel> TargetProperty {
        get {
            return this._TargetProperty;
        }
    }
    
    public virtual PlayerViewModel Target {
        get {
            return _TargetProperty.Value;
        }
        set {
            _TargetProperty.Value = value;
            if (value != null) value.ParentTryEntry = this;
        }
    }
    
    public virtual P<Single> PathLengthProperty {
        get {
            return this._PathLengthProperty;
        }
    }
    
    public virtual Single PathLength {
        get {
            return _PathLengthProperty.Value;
        }
        set {
            _PathLengthProperty.Value = value;
        }
    }
    
    public virtual CommandWithSender<TryEntryViewModel> Reset {
        get {
            return _Reset;
        }
        set {
            _Reset = value;
        }
    }
    
    public virtual LevelRootViewModel ParentLevelRoot {
        get {
            return this._ParentLevelRoot;
        }
        set {
            _ParentLevelRoot = value;
        }
    }
    
    protected override void WireCommands(Controller controller) {
        var tryEntry = controller as TryEntryControllerBase;
        this.Reset = new CommandWithSender<TryEntryViewModel>(this, tryEntry.Reset);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
        stream.SerializeInt("Number", this.Number);
		if (stream.DeepSerialize) stream.SerializeObject("Target", this.Target);
        stream.SerializeFloat("PathLength", this.PathLength);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
        		this.Number = stream.DeserializeInt("Number");;
		if (stream.DeepSerialize) this.Target = stream.DeserializeObject<PlayerViewModel>("Target");
        		this.PathLength = stream.DeserializeFloat("PathLength");;
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
        list.Add(new ViewModelPropertyInfo(_NumberProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_TargetProperty, true, false, false));
        list.Add(new ViewModelPropertyInfo(_PathLengthProperty, false, false, false));
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
        list.Add(new ViewModelCommandInfo("Reset", Reset) { ParameterType = typeof(void) });
    }
}

[DiagramInfoAttribute("GraviPath")]
public class EditorRootViewModelBase : ViewModel {
    
    public P<UniverseViewModel> _CurrentUniverseProperty;
    
    public P<NewUniverseSubEditorViewModel> _NewUniverseDataProperty;
    
    public P<Boolean> _IsUniverseDirtyProperty;
    
    public P<AddUniverseObjectSubEditorViewModel> _AddUniverseObjectSubEditorProperty;
    
    public ModelCollection<UniverseViewModel> _AvailableUniversesProperty;
    
    protected CommandWithSender<EditorRootViewModel> _ToMenu;
    
    protected CommandWithSenderAndArgument<EditorRootViewModel, UniverseViewModel> _LoadUniverse;
    
    protected CommandWithSender<EditorRootViewModel> _CreateNewUniverse;
    
    protected CommandWithSender<EditorRootViewModel> _ToggleNewUniverseSubEditor;
    
    protected CommandWithSender<EditorRootViewModel> _SaveCurrentUniverse;
    
    protected CommandWithSenderAndArgument<EditorRootViewModel, UniverseObjectDescriptor> _AddUniverseObject;
    
    protected CommandWithSenderAndArgument<EditorRootViewModel, Boolean> _SwitchUniverseObjectSubEditor;
    
    public EditorRootViewModelBase(EditorRootControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public EditorRootViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
        _CurrentUniverseProperty = new P<UniverseViewModel>(this, "CurrentUniverse");
        _NewUniverseDataProperty = new P<NewUniverseSubEditorViewModel>(this, "NewUniverseData");
        _IsUniverseDirtyProperty = new P<Boolean>(this, "IsUniverseDirty");
        _AddUniverseObjectSubEditorProperty = new P<AddUniverseObjectSubEditorViewModel>(this, "AddUniverseObjectSubEditor");
        _AvailableUniversesProperty = new ModelCollection<UniverseViewModel>(this, "AvailableUniverses");
        _AvailableUniversesProperty.CollectionChanged += AvailableUniversesCollectionChanged;
    }
    
    protected virtual void AvailableUniversesCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs args) {
    }
}

public partial class EditorRootViewModel : EditorRootViewModelBase {
    
    public EditorRootViewModel(EditorRootControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public EditorRootViewModel() : 
            base() {
    }
    
    public virtual P<UniverseViewModel> CurrentUniverseProperty {
        get {
            return this._CurrentUniverseProperty;
        }
    }
    
    public virtual UniverseViewModel CurrentUniverse {
        get {
            return _CurrentUniverseProperty.Value;
        }
        set {
            _CurrentUniverseProperty.Value = value;
            if (value != null) value.ParentEditorRoot = this;
        }
    }
    
    public virtual P<NewUniverseSubEditorViewModel> NewUniverseDataProperty {
        get {
            return this._NewUniverseDataProperty;
        }
    }
    
    public virtual NewUniverseSubEditorViewModel NewUniverseData {
        get {
            return _NewUniverseDataProperty.Value;
        }
        set {
            _NewUniverseDataProperty.Value = value;
            if (value != null) value.ParentEditorRoot = this;
        }
    }
    
    public virtual P<Boolean> IsUniverseDirtyProperty {
        get {
            return this._IsUniverseDirtyProperty;
        }
    }
    
    public virtual Boolean IsUniverseDirty {
        get {
            return _IsUniverseDirtyProperty.Value;
        }
        set {
            _IsUniverseDirtyProperty.Value = value;
        }
    }
    
    public virtual P<AddUniverseObjectSubEditorViewModel> AddUniverseObjectSubEditorProperty {
        get {
            return this._AddUniverseObjectSubEditorProperty;
        }
    }
    
    public virtual AddUniverseObjectSubEditorViewModel AddUniverseObjectSubEditor {
        get {
            return _AddUniverseObjectSubEditorProperty.Value;
        }
        set {
            _AddUniverseObjectSubEditorProperty.Value = value;
            if (value != null) value.ParentEditorRoot = this;
        }
    }
    
    public virtual ModelCollection<UniverseViewModel> AvailableUniverses {
        get {
            return this._AvailableUniversesProperty;
        }
    }
    
    public virtual CommandWithSender<EditorRootViewModel> ToMenu {
        get {
            return _ToMenu;
        }
        set {
            _ToMenu = value;
        }
    }
    
    public virtual CommandWithSenderAndArgument<EditorRootViewModel, UniverseViewModel> LoadUniverse {
        get {
            return _LoadUniverse;
        }
        set {
            _LoadUniverse = value;
        }
    }
    
    public virtual CommandWithSender<EditorRootViewModel> CreateNewUniverse {
        get {
            return _CreateNewUniverse;
        }
        set {
            _CreateNewUniverse = value;
        }
    }
    
    public virtual CommandWithSender<EditorRootViewModel> ToggleNewUniverseSubEditor {
        get {
            return _ToggleNewUniverseSubEditor;
        }
        set {
            _ToggleNewUniverseSubEditor = value;
        }
    }
    
    public virtual CommandWithSender<EditorRootViewModel> SaveCurrentUniverse {
        get {
            return _SaveCurrentUniverse;
        }
        set {
            _SaveCurrentUniverse = value;
        }
    }
    
    public virtual CommandWithSenderAndArgument<EditorRootViewModel, UniverseObjectDescriptor> AddUniverseObject {
        get {
            return _AddUniverseObject;
        }
        set {
            _AddUniverseObject = value;
        }
    }
    
    public virtual CommandWithSenderAndArgument<EditorRootViewModel, Boolean> SwitchUniverseObjectSubEditor {
        get {
            return _SwitchUniverseObjectSubEditor;
        }
        set {
            _SwitchUniverseObjectSubEditor = value;
        }
    }
    
    protected override void WireCommands(Controller controller) {
        var editorRoot = controller as EditorRootControllerBase;
        this.ToMenu = new CommandWithSender<EditorRootViewModel>(this, editorRoot.ToMenu);
        this.LoadUniverse = new CommandWithSenderAndArgument<EditorRootViewModel, UniverseViewModel>(this, editorRoot.LoadUniverse);
        this.CreateNewUniverse = new CommandWithSender<EditorRootViewModel>(this, editorRoot.CreateNewUniverse);
        this.ToggleNewUniverseSubEditor = new CommandWithSender<EditorRootViewModel>(this, editorRoot.ToggleNewUniverseSubEditor);
        this.SaveCurrentUniverse = new CommandWithSender<EditorRootViewModel>(this, editorRoot.SaveCurrentUniverse);
        this.AddUniverseObject = new CommandWithSenderAndArgument<EditorRootViewModel, UniverseObjectDescriptor>(this, editorRoot.AddUniverseObject);
        this.SwitchUniverseObjectSubEditor = new CommandWithSenderAndArgument<EditorRootViewModel, Boolean>(this, editorRoot.SwitchUniverseObjectSubEditor);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
		if (stream.DeepSerialize) stream.SerializeObject("CurrentUniverse", this.CurrentUniverse);
		if (stream.DeepSerialize) stream.SerializeObject("NewUniverseData", this.NewUniverseData);
        stream.SerializeBool("IsUniverseDirty", this.IsUniverseDirty);
		if (stream.DeepSerialize) stream.SerializeObject("AddUniverseObjectSubEditor", this.AddUniverseObjectSubEditor);
        if (stream.DeepSerialize) stream.SerializeArray("AvailableUniverses", this.AvailableUniverses);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
		if (stream.DeepSerialize) this.CurrentUniverse = stream.DeserializeObject<UniverseViewModel>("CurrentUniverse");
		if (stream.DeepSerialize) this.NewUniverseData = stream.DeserializeObject<NewUniverseSubEditorViewModel>("NewUniverseData");
        		this.IsUniverseDirty = stream.DeserializeBool("IsUniverseDirty");;
		if (stream.DeepSerialize) this.AddUniverseObjectSubEditor = stream.DeserializeObject<AddUniverseObjectSubEditorViewModel>("AddUniverseObjectSubEditor");
if (stream.DeepSerialize) {
        this.AvailableUniverses.Clear();
        this.AvailableUniverses.AddRange(stream.DeserializeObjectArray<UniverseViewModel>("AvailableUniverses"));
}
    }
    
    public override void Unbind() {
        base.Unbind();
        _AvailableUniversesProperty.CollectionChanged -= AvailableUniversesCollectionChanged;
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
        list.Add(new ViewModelPropertyInfo(_CurrentUniverseProperty, true, false, false));
        list.Add(new ViewModelPropertyInfo(_NewUniverseDataProperty, true, false, false));
        list.Add(new ViewModelPropertyInfo(_IsUniverseDirtyProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_AddUniverseObjectSubEditorProperty, true, false, false));
        list.Add(new ViewModelPropertyInfo(_AvailableUniversesProperty, true, true, false));
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
        list.Add(new ViewModelCommandInfo("ToMenu", ToMenu) { ParameterType = typeof(void) });
        list.Add(new ViewModelCommandInfo("LoadUniverse", LoadUniverse) { ParameterType = typeof(UniverseViewModel) });
        list.Add(new ViewModelCommandInfo("CreateNewUniverse", CreateNewUniverse) { ParameterType = typeof(void) });
        list.Add(new ViewModelCommandInfo("ToggleNewUniverseSubEditor", ToggleNewUniverseSubEditor) { ParameterType = typeof(void) });
        list.Add(new ViewModelCommandInfo("SaveCurrentUniverse", SaveCurrentUniverse) { ParameterType = typeof(void) });
        list.Add(new ViewModelCommandInfo("AddUniverseObject", AddUniverseObject) { ParameterType = typeof(UniverseObjectDescriptor) });
        list.Add(new ViewModelCommandInfo("SwitchUniverseObjectSubEditor", SwitchUniverseObjectSubEditor) { ParameterType = typeof(Boolean) });
    }
    
    protected override void AvailableUniversesCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs args) {
        foreach (var item in args.NewItems.OfType<UniverseViewModel>()) item.ParentEditorRoot = this;;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class UniverseViewModelBase : ViewModel {
    
    public P<String> _NameProperty;
    
    public P<String> _AuthorProperty;
    
    public P<Boolean> _IsEditableProperty;
    
    public ModelCollection<UniverseObjectViewModel> _ObjectsProperty;
    
    protected CommandWithSender<UniverseViewModel> _Reset;
    
    protected CommandWithSender<UniverseViewModel> _Save;
    
    public UniverseViewModelBase(UniverseControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public UniverseViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
        _NameProperty = new P<String>(this, "Name");
        _AuthorProperty = new P<String>(this, "Author");
        _IsEditableProperty = new P<Boolean>(this, "IsEditable");
        _ObjectsProperty = new ModelCollection<UniverseObjectViewModel>(this, "Objects");
        _ObjectsProperty.CollectionChanged += ObjectsCollectionChanged;
    }
    
    protected virtual void ObjectsCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs args) {
    }
}

public partial class UniverseViewModel : UniverseViewModelBase {
    
    private MenuRootViewModel _ParentMenuRoot;
    
    private LevelRootViewModel _ParentLevelRoot;
    
    private EditorRootViewModel _ParentEditorRoot;
    
    public UniverseViewModel(UniverseControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public UniverseViewModel() : 
            base() {
    }
    
    public virtual P<String> NameProperty {
        get {
            return this._NameProperty;
        }
    }
    
    public virtual String Name {
        get {
            return _NameProperty.Value;
        }
        set {
            _NameProperty.Value = value;
        }
    }
    
    public virtual P<String> AuthorProperty {
        get {
            return this._AuthorProperty;
        }
    }
    
    public virtual String Author {
        get {
            return _AuthorProperty.Value;
        }
        set {
            _AuthorProperty.Value = value;
        }
    }
    
    public virtual P<Boolean> IsEditableProperty {
        get {
            return this._IsEditableProperty;
        }
    }
    
    public virtual Boolean IsEditable {
        get {
            return _IsEditableProperty.Value;
        }
        set {
            _IsEditableProperty.Value = value;
        }
    }
    
    public virtual ModelCollection<UniverseObjectViewModel> Objects {
        get {
            return this._ObjectsProperty;
        }
    }
    
    public virtual CommandWithSender<UniverseViewModel> Reset {
        get {
            return _Reset;
        }
        set {
            _Reset = value;
        }
    }
    
    public virtual CommandWithSender<UniverseViewModel> Save {
        get {
            return _Save;
        }
        set {
            _Save = value;
        }
    }
    
    public virtual MenuRootViewModel ParentMenuRoot {
        get {
            return this._ParentMenuRoot;
        }
        set {
            _ParentMenuRoot = value;
        }
    }
    
    public virtual LevelRootViewModel ParentLevelRoot {
        get {
            return this._ParentLevelRoot;
        }
        set {
            _ParentLevelRoot = value;
        }
    }
    
    public virtual EditorRootViewModel ParentEditorRoot {
        get {
            return this._ParentEditorRoot;
        }
        set {
            _ParentEditorRoot = value;
        }
    }
    
    protected override void WireCommands(Controller controller) {
        var universe = controller as UniverseControllerBase;
        this.Reset = new CommandWithSender<UniverseViewModel>(this, universe.Reset);
        this.Save = new CommandWithSender<UniverseViewModel>(this, universe.Save);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
        stream.SerializeString("Name", this.Name);
        stream.SerializeString("Author", this.Author);
        stream.SerializeBool("IsEditable", this.IsEditable);
        if (stream.DeepSerialize) stream.SerializeArray("Objects", this.Objects);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
        		this.Name = stream.DeserializeString("Name");;
        		this.Author = stream.DeserializeString("Author");;
        		this.IsEditable = stream.DeserializeBool("IsEditable");;
if (stream.DeepSerialize) {
        this.Objects.Clear();
        this.Objects.AddRange(stream.DeserializeObjectArray<UniverseObjectViewModel>("Objects"));
}
    }
    
    public override void Unbind() {
        base.Unbind();
        _ObjectsProperty.CollectionChanged -= ObjectsCollectionChanged;
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
        list.Add(new ViewModelPropertyInfo(_NameProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_AuthorProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_IsEditableProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_ObjectsProperty, true, true, false));
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
        list.Add(new ViewModelCommandInfo("Reset", Reset) { ParameterType = typeof(void) });
        list.Add(new ViewModelCommandInfo("Save", Save) { ParameterType = typeof(void) });
    }
    
    protected override void ObjectsCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs args) {
        foreach (var item in args.NewItems.OfType<UniverseObjectViewModel>()) item.ParentUniverse = this;;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class UniverseObjectViewModelBase : ViewModel {
    
    public P<Vector3> _StartPositionProperty;
    
    public P<Vector3> _StartRotationProperty;
    
    public P<Vector3> _StartScaleProperty;
    
    public P<Vector3> _PositionProperty;
    
    public P<Vector3> _RotationProperty;
    
    public P<Vector3> _ScaleProperty;
    
    public P<Boolean> _IsEditableProperty;
    
    protected CommandWithSender<UniverseObjectViewModel> _Reset;
    
    protected CommandWithSender<UniverseObjectViewModel> _Save;
    
    public UniverseObjectViewModelBase(UniverseObjectControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public UniverseObjectViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
        _StartPositionProperty = new P<Vector3>(this, "StartPosition");
        _StartRotationProperty = new P<Vector3>(this, "StartRotation");
        _StartScaleProperty = new P<Vector3>(this, "StartScale");
        _PositionProperty = new P<Vector3>(this, "Position");
        _RotationProperty = new P<Vector3>(this, "Rotation");
        _ScaleProperty = new P<Vector3>(this, "Scale");
        _IsEditableProperty = new P<Boolean>(this, "IsEditable");
    }
}

public partial class UniverseObjectViewModel : UniverseObjectViewModelBase {
    
    private UniverseViewModel _ParentUniverse;
    
    public UniverseObjectViewModel(UniverseObjectControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public UniverseObjectViewModel() : 
            base() {
    }
    
    public virtual P<Vector3> StartPositionProperty {
        get {
            return this._StartPositionProperty;
        }
    }
    
    public virtual Vector3 StartPosition {
        get {
            return _StartPositionProperty.Value;
        }
        set {
            _StartPositionProperty.Value = value;
        }
    }
    
    public virtual P<Vector3> StartRotationProperty {
        get {
            return this._StartRotationProperty;
        }
    }
    
    public virtual Vector3 StartRotation {
        get {
            return _StartRotationProperty.Value;
        }
        set {
            _StartRotationProperty.Value = value;
        }
    }
    
    public virtual P<Vector3> StartScaleProperty {
        get {
            return this._StartScaleProperty;
        }
    }
    
    public virtual Vector3 StartScale {
        get {
            return _StartScaleProperty.Value;
        }
        set {
            _StartScaleProperty.Value = value;
        }
    }
    
    public virtual P<Vector3> PositionProperty {
        get {
            return this._PositionProperty;
        }
    }
    
    public virtual Vector3 Position {
        get {
            return _PositionProperty.Value;
        }
        set {
            _PositionProperty.Value = value;
        }
    }
    
    public virtual P<Vector3> RotationProperty {
        get {
            return this._RotationProperty;
        }
    }
    
    public virtual Vector3 Rotation {
        get {
            return _RotationProperty.Value;
        }
        set {
            _RotationProperty.Value = value;
        }
    }
    
    public virtual P<Vector3> ScaleProperty {
        get {
            return this._ScaleProperty;
        }
    }
    
    public virtual Vector3 Scale {
        get {
            return _ScaleProperty.Value;
        }
        set {
            _ScaleProperty.Value = value;
        }
    }
    
    public virtual P<Boolean> IsEditableProperty {
        get {
            return this._IsEditableProperty;
        }
    }
    
    public virtual Boolean IsEditable {
        get {
            return _IsEditableProperty.Value;
        }
        set {
            _IsEditableProperty.Value = value;
        }
    }
    
    public virtual CommandWithSender<UniverseObjectViewModel> Reset {
        get {
            return _Reset;
        }
        set {
            _Reset = value;
        }
    }
    
    public virtual CommandWithSender<UniverseObjectViewModel> Save {
        get {
            return _Save;
        }
        set {
            _Save = value;
        }
    }
    
    public virtual UniverseViewModel ParentUniverse {
        get {
            return this._ParentUniverse;
        }
        set {
            _ParentUniverse = value;
        }
    }
    
    protected override void WireCommands(Controller controller) {
        var universeObject = controller as UniverseObjectControllerBase;
        this.Reset = new CommandWithSender<UniverseObjectViewModel>(this, universeObject.Reset);
        this.Save = new CommandWithSender<UniverseObjectViewModel>(this, universeObject.Save);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
        stream.SerializeVector3("StartPosition", this.StartPosition);
        stream.SerializeVector3("StartRotation", this.StartRotation);
        stream.SerializeVector3("StartScale", this.StartScale);
        stream.SerializeVector3("Position", this.Position);
        stream.SerializeVector3("Rotation", this.Rotation);
        stream.SerializeVector3("Scale", this.Scale);
        stream.SerializeBool("IsEditable", this.IsEditable);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
        		this.StartPosition = stream.DeserializeVector3("StartPosition");;
        		this.StartRotation = stream.DeserializeVector3("StartRotation");;
        		this.StartScale = stream.DeserializeVector3("StartScale");;
        		this.Position = stream.DeserializeVector3("Position");;
        		this.Rotation = stream.DeserializeVector3("Rotation");;
        		this.Scale = stream.DeserializeVector3("Scale");;
        		this.IsEditable = stream.DeserializeBool("IsEditable");;
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
        list.Add(new ViewModelPropertyInfo(_StartPositionProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_StartRotationProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_StartScaleProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_PositionProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_RotationProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_ScaleProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_IsEditableProperty, false, false, false));
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
        list.Add(new ViewModelCommandInfo("Reset", Reset) { ParameterType = typeof(void) });
        list.Add(new ViewModelCommandInfo("Save", Save) { ParameterType = typeof(void) });
    }
}

[DiagramInfoAttribute("GraviPath")]
public class ZoneViewModelBase : UniverseObjectViewModel {
    
    public ZoneViewModelBase(ZoneControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public ZoneViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class ZoneViewModel : ZoneViewModelBase {
    
    private PlayerViewModel _ParentPlayer;
    
    public ZoneViewModel(ZoneControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public ZoneViewModel() : 
            base() {
    }
    
    public virtual PlayerViewModel ParentPlayer {
        get {
            return this._ParentPlayer;
        }
        set {
            _ParentPlayer = value;
        }
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class GravityObjectViewModelBase : UniverseObjectViewModel {
    
    public GravityObjectViewModelBase(GravityObjectControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public GravityObjectViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class GravityObjectViewModel : GravityObjectViewModelBase {
    
    public GravityObjectViewModel(GravityObjectControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public GravityObjectViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class PlanetViewModelBase : GravityObjectViewModel {
    
    public PlanetViewModelBase(PlanetControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public PlanetViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class PlanetViewModel : PlanetViewModelBase {
    
    public PlanetViewModel(PlanetControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public PlanetViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class AsteroidViewModelBase : GravityObjectViewModel {
    
    public AsteroidViewModelBase(AsteroidControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public AsteroidViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class AsteroidViewModel : AsteroidViewModelBase {
    
    public AsteroidViewModel(AsteroidControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public AsteroidViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class BlackholeViewModelBase : GravityObjectViewModel {
    
    public BlackholeViewModelBase(BlackholeControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public BlackholeViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class BlackholeViewModel : BlackholeViewModelBase {
    
    public BlackholeViewModel(BlackholeControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public BlackholeViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class MiniObjectViewModelBase : UniverseObjectViewModel {
    
    public MiniObjectViewModelBase(MiniObjectControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public MiniObjectViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class MiniObjectViewModel : MiniObjectViewModelBase {
    
    public MiniObjectViewModel(MiniObjectControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public MiniObjectViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class SimplePlanet1ViewModelBase : PlanetViewModel {
    
    public SimplePlanet1ViewModelBase(SimplePlanet1ControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public SimplePlanet1ViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class SimplePlanet1ViewModel : SimplePlanet1ViewModelBase {
    
    public SimplePlanet1ViewModel(SimplePlanet1ControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public SimplePlanet1ViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class SimplePlanet2ViewModelBase : PlanetViewModel {
    
    public SimplePlanet2ViewModelBase(SimplePlanet2ControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public SimplePlanet2ViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class SimplePlanet2ViewModel : SimplePlanet2ViewModelBase {
    
    public SimplePlanet2ViewModel(SimplePlanet2ControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public SimplePlanet2ViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class SimpleAsteroid1ViewModelBase : AsteroidViewModel {
    
    public SimpleAsteroid1ViewModelBase(SimpleAsteroid1ControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public SimpleAsteroid1ViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class SimpleAsteroid1ViewModel : SimpleAsteroid1ViewModelBase {
    
    public SimpleAsteroid1ViewModel(SimpleAsteroid1ControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public SimpleAsteroid1ViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class SimpleAsteroid2ViewModelBase : AsteroidViewModel {
    
    public SimpleAsteroid2ViewModelBase(SimpleAsteroid2ControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public SimpleAsteroid2ViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class SimpleAsteroid2ViewModel : SimpleAsteroid2ViewModelBase {
    
    public SimpleAsteroid2ViewModel(SimpleAsteroid2ControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public SimpleAsteroid2ViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class SimpleAsteroid3ViewModelBase : AsteroidViewModel {
    
    public SimpleAsteroid3ViewModelBase(SimpleAsteroid3ControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public SimpleAsteroid3ViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class SimpleAsteroid3ViewModel : SimpleAsteroid3ViewModelBase {
    
    public SimpleAsteroid3ViewModel(SimpleAsteroid3ControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public SimpleAsteroid3ViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class SimpleAsteroid4ViewModelBase : AsteroidViewModel {
    
    public SimpleAsteroid4ViewModelBase(SimpleAsteroid4ControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public SimpleAsteroid4ViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class SimpleAsteroid4ViewModel : SimpleAsteroid4ViewModelBase {
    
    public SimpleAsteroid4ViewModel(SimpleAsteroid4ControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public SimpleAsteroid4ViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class SimpleBlackhole1ViewModelBase : BlackholeViewModel {
    
    public SimpleBlackhole1ViewModelBase(SimpleBlackhole1ControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public SimpleBlackhole1ViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class SimpleBlackhole1ViewModel : SimpleBlackhole1ViewModelBase {
    
    public SimpleBlackhole1ViewModel(SimpleBlackhole1ControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public SimpleBlackhole1ViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class SpaceGarbageAreaViewModelBase : UniverseObjectViewModel {
    
    public ModelCollection<String> _CollectionProperty;
    
    public SpaceGarbageAreaViewModelBase(SpaceGarbageAreaControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public SpaceGarbageAreaViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
        _CollectionProperty = new ModelCollection<String>(this, "Collection");
    }
}

public partial class SpaceGarbageAreaViewModel : SpaceGarbageAreaViewModelBase {
    
    public SpaceGarbageAreaViewModel(SpaceGarbageAreaControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public SpaceGarbageAreaViewModel() : 
            base() {
    }
    
    public virtual ModelCollection<String> Collection {
        get {
            return this._CollectionProperty;
        }
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
        list.Add(new ViewModelPropertyInfo(_CollectionProperty, false, true, false));
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class UniverseRepositoryViewModelBase : ViewModel {
    
    public UniverseRepositoryViewModelBase(UniverseRepositoryControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public UniverseRepositoryViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class UniverseRepositoryViewModel : UniverseRepositoryViewModelBase {
    
    public UniverseRepositoryViewModel(UniverseRepositoryControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public UniverseRepositoryViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class NewUniverseSubEditorViewModelBase : ViewModel {
    
    private IDisposable _IsValidDisposable;
    
    public P<String> _NameProperty;
    
    public P<String> _DescriptionProperty;
    
    public P<Boolean> _IsActiveProperty;
    
    public P<Boolean> _IsValidProperty;
    
    protected CommandWithSender<NewUniverseSubEditorViewModel> _CreateUniverse;
    
    public NewUniverseSubEditorViewModelBase(NewUniverseSubEditorControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public NewUniverseSubEditorViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
        _NameProperty = new P<String>(this, "Name");
        _DescriptionProperty = new P<String>(this, "Description");
        _IsActiveProperty = new P<Boolean>(this, "IsActive");
        _IsValidProperty = new P<Boolean>(this, "IsValid");
        this.ResetIsValid();
    }
    
    public virtual void ResetIsValid() {
        if (_IsValidDisposable != null) _IsValidDisposable.Dispose();
        _IsValidDisposable = _IsValidProperty.ToComputed( ComputeIsValid, this.GetIsValidDependents().ToArray() ).DisposeWith(this);
    }
    
    public virtual Boolean ComputeIsValid() {
        return default(Boolean);
    }
    
    public virtual IEnumerable<IObservableProperty> GetIsValidDependents() {
        yield return _NameProperty;
        yield return _DescriptionProperty;
        yield break;
    }
}

public partial class NewUniverseSubEditorViewModel : NewUniverseSubEditorViewModelBase {
    
    private EditorRootViewModel _ParentEditorRoot;
    
    public NewUniverseSubEditorViewModel(NewUniverseSubEditorControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public NewUniverseSubEditorViewModel() : 
            base() {
    }
    
    public virtual P<String> NameProperty {
        get {
            return this._NameProperty;
        }
    }
    
    public virtual String Name {
        get {
            return _NameProperty.Value;
        }
        set {
            _NameProperty.Value = value;
        }
    }
    
    public virtual P<String> DescriptionProperty {
        get {
            return this._DescriptionProperty;
        }
    }
    
    public virtual String Description {
        get {
            return _DescriptionProperty.Value;
        }
        set {
            _DescriptionProperty.Value = value;
        }
    }
    
    public virtual P<Boolean> IsActiveProperty {
        get {
            return this._IsActiveProperty;
        }
    }
    
    public virtual Boolean IsActive {
        get {
            return _IsActiveProperty.Value;
        }
        set {
            _IsActiveProperty.Value = value;
        }
    }
    
    public virtual P<Boolean> IsValidProperty {
        get {
            return this._IsValidProperty;
        }
    }
    
    public virtual Boolean IsValid {
        get {
            return _IsValidProperty.Value;
        }
        set {
            _IsValidProperty.Value = value;
        }
    }
    
    public virtual CommandWithSender<NewUniverseSubEditorViewModel> CreateUniverse {
        get {
            return _CreateUniverse;
        }
        set {
            _CreateUniverse = value;
        }
    }
    
    public virtual EditorRootViewModel ParentEditorRoot {
        get {
            return this._ParentEditorRoot;
        }
        set {
            _ParentEditorRoot = value;
        }
    }
    
    protected override void WireCommands(Controller controller) {
        var newUniverseSubEditor = controller as NewUniverseSubEditorControllerBase;
        this.CreateUniverse = new CommandWithSender<NewUniverseSubEditorViewModel>(this, newUniverseSubEditor.CreateUniverse);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
        stream.SerializeString("Name", this.Name);
        stream.SerializeString("Description", this.Description);
        stream.SerializeBool("IsActive", this.IsActive);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
        		this.Name = stream.DeserializeString("Name");;
        		this.Description = stream.DeserializeString("Description");;
        		this.IsActive = stream.DeserializeBool("IsActive");;
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
        list.Add(new ViewModelPropertyInfo(_NameProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_DescriptionProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_IsActiveProperty, false, false, false));
        list.Add(new ViewModelPropertyInfo(_IsValidProperty, false, false, false, true));
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
        list.Add(new ViewModelCommandInfo("CreateUniverse", CreateUniverse) { ParameterType = typeof(void) });
    }
}

[DiagramInfoAttribute("GraviPath")]
public class AddUniverseObjectSubEditorViewModelBase : ViewModel {
    
    public P<Boolean> _IsActiveProperty;
    
    protected CommandWithSenderAndArgument<AddUniverseObjectSubEditorViewModel, UniverseObjectDescriptor> _Add;
    
    public AddUniverseObjectSubEditorViewModelBase(AddUniverseObjectSubEditorControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public AddUniverseObjectSubEditorViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
        _IsActiveProperty = new P<Boolean>(this, "IsActive");
    }
}

public partial class AddUniverseObjectSubEditorViewModel : AddUniverseObjectSubEditorViewModelBase {
    
    private EditorRootViewModel _ParentEditorRoot;
    
    public AddUniverseObjectSubEditorViewModel(AddUniverseObjectSubEditorControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public AddUniverseObjectSubEditorViewModel() : 
            base() {
    }
    
    public virtual P<Boolean> IsActiveProperty {
        get {
            return this._IsActiveProperty;
        }
    }
    
    public virtual Boolean IsActive {
        get {
            return _IsActiveProperty.Value;
        }
        set {
            _IsActiveProperty.Value = value;
        }
    }
    
    public virtual CommandWithSenderAndArgument<AddUniverseObjectSubEditorViewModel, UniverseObjectDescriptor> Add {
        get {
            return _Add;
        }
        set {
            _Add = value;
        }
    }
    
    public virtual EditorRootViewModel ParentEditorRoot {
        get {
            return this._ParentEditorRoot;
        }
        set {
            _ParentEditorRoot = value;
        }
    }
    
    protected override void WireCommands(Controller controller) {
        var addUniverseObjectSubEditor = controller as AddUniverseObjectSubEditorControllerBase;
        this.Add = new CommandWithSenderAndArgument<AddUniverseObjectSubEditorViewModel, UniverseObjectDescriptor>(this, addUniverseObjectSubEditor.Add);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
        stream.SerializeBool("IsActive", this.IsActive);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
        		this.IsActive = stream.DeserializeBool("IsActive");;
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
        list.Add(new ViewModelPropertyInfo(_IsActiveProperty, false, false, false));
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
        list.Add(new ViewModelCommandInfo("Add", Add) { ParameterType = typeof(UniverseObjectDescriptor) });
    }
}

[DiagramInfoAttribute("GraviPath")]
public class StartZoneViewModelBase : ZoneViewModel {
    
    public StartZoneViewModelBase(StartZoneControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public StartZoneViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class StartZoneViewModel : StartZoneViewModelBase {
    
    public StartZoneViewModel(StartZoneControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public StartZoneViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class WinZoneViewModelBase : ZoneViewModel {
    
    public WinZoneViewModelBase(WinZoneControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public WinZoneViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class WinZoneViewModel : WinZoneViewModelBase {
    
    public WinZoneViewModel(WinZoneControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public WinZoneViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class PickupableViewModelBase : UniverseObjectViewModel {
    
    public P<Boolean> _IsActiveProperty;
    
    protected CommandWithSender<PickupableViewModel> _PickUp;
    
    public PickupableViewModelBase(PickupableControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public PickupableViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
        _IsActiveProperty = new P<Boolean>(this, "IsActive");
    }
}

public partial class PickupableViewModel : PickupableViewModelBase {
    
    private PlayerViewModel _ParentPlayer;
    
    public PickupableViewModel(PickupableControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public PickupableViewModel() : 
            base() {
    }
    
    public virtual P<Boolean> IsActiveProperty {
        get {
            return this._IsActiveProperty;
        }
    }
    
    public virtual Boolean IsActive {
        get {
            return _IsActiveProperty.Value;
        }
        set {
            _IsActiveProperty.Value = value;
        }
    }
    
    public virtual CommandWithSender<PickupableViewModel> PickUp {
        get {
            return _PickUp;
        }
        set {
            _PickUp = value;
        }
    }
    
    public virtual PlayerViewModel ParentPlayer {
        get {
            return this._ParentPlayer;
        }
        set {
            _ParentPlayer = value;
        }
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
        var pickupable = controller as PickupableControllerBase;
        this.PickUp = new CommandWithSender<PickupableViewModel>(this, pickupable.PickUp);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
        stream.SerializeBool("IsActive", this.IsActive);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
        		this.IsActive = stream.DeserializeBool("IsActive");;
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
        list.Add(new ViewModelPropertyInfo(_IsActiveProperty, false, false, false));
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
        list.Add(new ViewModelCommandInfo("PickUp", PickUp) { ParameterType = typeof(void) });
    }
}

[DiagramInfoAttribute("GraviPath")]
public class ScorePointViewModelBase : PickupableViewModel {
    
    public ScorePointViewModelBase(ScorePointControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public ScorePointViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class ScorePointViewModel : ScorePointViewModelBase {
    
    public ScorePointViewModel(ScorePointControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public ScorePointViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class PowerUpPickupableViewModelBase : PickupableViewModel {
    
    public PowerUpPickupableViewModelBase(PowerUpPickupableControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public PowerUpPickupableViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class PowerUpPickupableViewModel : PowerUpPickupableViewModelBase {
    
    public PowerUpPickupableViewModel(PowerUpPickupableControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public PowerUpPickupableViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

[DiagramInfoAttribute("GraviPath")]
public class AcceleratorPowerUpViewModelBase : PowerUpPickupableViewModel {
    
    public AcceleratorPowerUpViewModelBase(AcceleratorPowerUpControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public AcceleratorPowerUpViewModelBase() : 
            base() {
    }
    
    public override void Bind() {
        base.Bind();
    }
}

public partial class AcceleratorPowerUpViewModel : AcceleratorPowerUpViewModelBase {
    
    public AcceleratorPowerUpViewModel(AcceleratorPowerUpControllerBase controller, bool initialize = true) : 
            base(controller, initialize) {
    }
    
    public AcceleratorPowerUpViewModel() : 
            base() {
    }
    
    protected override void WireCommands(Controller controller) {
        base.WireCommands(controller);
    }
    
    public override void Write(ISerializerStream stream) {
		base.Write(stream);
    }
    
    public override void Read(ISerializerStream stream) {
		base.Read(stream);
    }
    
    public override void Unbind() {
        base.Unbind();
    }
    
    protected override void FillProperties(List<ViewModelPropertyInfo> list) {
        base.FillProperties(list);;
    }
    
    protected override void FillCommands(List<ViewModelCommandInfo> list) {
        base.FillCommands(list);;
    }
}

public enum UniverseObjectType {
    
    Planet1,
    
    Planet2,
    
    Asteroid1,
    
    Asteroid2,
    
    Asteroid3,
    
    Asteroid4,
    
    StartZone,
    
    WinZone,
    
    Star,
    
    Accelerator,
}

public enum UniverseListUpdateType {
    
    Latest,
    
    ByAuthor,
}
