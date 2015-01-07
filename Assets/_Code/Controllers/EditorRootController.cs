using System;
using System.Diagnostics;
using System.Linq;
using UniRx;

public class EditorRootController : EditorRootControllerBase
{
    [Inject] public AcceleratorPowerUpController AcceleratorPowerUpController;
    [Inject] public ScorePointController ScorePointController;
    [Inject] public SimpleAsteroid1Controller SimpleAsteroid1Controller;
    [Inject] public SimpleAsteroid2Controller SimpleAsteroid2Controller;
    [Inject] public SimpleAsteroid3Controller SimpleAsteroid3Controller;
    [Inject] public SimpleAsteroid4Controller SimpleAsteroid4Controller;
    [Inject] public SimplePlanet1Controller SimplePlanet1Controller;
    [Inject] public SimplePlanet2Controller SimplePlanet2Controller;
    [Inject] public StartZoneController StartZoneController;
    [Inject] public WinZoneController WinZoneController;

    public override void InitializeEditorRoot(EditorRootViewModel editorRoot)
    {
        if (editorRoot.Initialized == true) return;
        editorRoot.Initialized = true;
        
        UnityEngine.Debug.Log("Initializing "+editorRoot.Identifier);

        editorRoot.NewUniverseDataProperty.Where(editor => editor != null)
            .Subscribe(editor => { NewUniverseEditorChanged(editorRoot, editor); });
        editorRoot.AddUniverseObjectSubEditorProperty.Where(editor => editor != null)
            .Subscribe(editor => { AddUniverseObjectEditorChanged(editorRoot, editor); });
        editorRoot.AvailableUniverses.Clear();
        editorRoot.CurrentUniverseProperty.Subscribe(
            uni =>
            {
                if (editorRoot.AddUniverseObjectSubEditor != null)
                    editorRoot.AddUniverseObjectSubEditor.IsActive = uni != null;
            });
        UniverseRepository.GetLatestPaged(10, 0).Subscribe(uni => { editorRoot.AvailableUniverses.Add(uni); });
    }

    private void AddUniverseObjectEditorChanged(EditorRootViewModel editorRoot,
        AddUniverseObjectSubEditorViewModel editor)
    {
        
        editorRoot.AddUniverseObjectSubEditor.Add.Subscribe(desc =>
        {
            //WHY DO I HAVE TO USE PARAMETER HERE insdeat of desc ????? HUH
            ExecuteCommand(editorRoot.AddUniverseObject, editorRoot.AddUniverseObjectSubEditor.Add.Parameter);
        }).DisposeWhenChanged(editorRoot.AddUniverseObjectSubEditorProperty);
    }

    private void NewUniverseEditorChanged(EditorRootViewModel editorRoot, NewUniverseSubEditorViewModel editor)
    {
        editorRoot.NewUniverseData.CreateUniverse.Subscribe(_ =>
        {
            ExecuteCommand(editorRoot.CreateNewUniverse);
            ExecuteCommand(editorRoot.ToggleNewUniverseSubEditor);
        }).DisposeWhenChanged(editorRoot.NewUniverseDataProperty);
    }

    public override void CreateNewUniverse(EditorRootViewModel editorRoot)
    {
        base.CreateNewUniverse(editorRoot);
        var newUniverseSubEditor = UniverseController.CreateUniverse();
        newUniverseSubEditor.IsEditable = true;
        newUniverseSubEditor.Name = editorRoot.NewUniverseData.Name;
        editorRoot.CurrentUniverse = newUniverseSubEditor;
    }

    public override void ToggleNewUniverseSubEditor(EditorRootViewModel editorRoot)
    {
        base.ToggleNewUniverseSubEditor(editorRoot);
        editorRoot.NewUniverseData.IsActive = !editorRoot.NewUniverseData.IsActive;
    }

    public override void SaveCurrentUniverse(EditorRootViewModel editorRoot)
    {
        base.SaveCurrentUniverse(editorRoot);

        var universe = editorRoot.CurrentUniverse;

        //Turn universe into snapshot

        ExecuteCommand(universe.Save);

        //TODO: Setup some sort of loading screen using ContinueWith

        //Save universe to the cloud
        UniverseRepository
            .SaveUniverse(editorRoot.CurrentUniverse)
            .Delay(TimeSpan.FromSeconds(1)) // Hack to fix parse delay
            .Subscribe(_ =>
            {
                editorRoot.ClearUniverses();
                UniverseRepository
                    .GetLatestPaged(10, 0)
                    .Subscribe(uni =>
                    {
                        editorRoot.AvailableUniverses.Add(uni);
                    });
            });
    }

    public override void LoadUniverse(EditorRootViewModel editorRoot, UniverseViewModel universe)
    {
        base.LoadUniverse(editorRoot, universe);
        universe.IsEditable = true;
        editorRoot.CurrentUniverse = universe;
    }

    public override void AddUniverseObject(EditorRootViewModel editorRoot, UniverseObjectDescriptor uObjectDescriptor)
    {
        base.AddUniverseObject(editorRoot, uObjectDescriptor);

        UniverseObjectViewModel uObject;

        switch (uObjectDescriptor.Type)
        {
            case UniverseObjectType.Planet1:
                uObject = SimplePlanet1Controller.CreateSimplePlanet1();
                break;
            case UniverseObjectType.Planet2:
                uObject = SimplePlanet2Controller.CreateSimplePlanet2();
                break;
            case UniverseObjectType.Asteroid1:
                uObject = SimpleAsteroid1Controller.CreateSimpleAsteroid1();
                break;
            case UniverseObjectType.Asteroid2:
                uObject = SimpleAsteroid2Controller.CreateSimpleAsteroid2();
                break;
            case UniverseObjectType.Asteroid3:
                uObject = SimpleAsteroid3Controller.CreateSimpleAsteroid3();
                break;
            case UniverseObjectType.Asteroid4:
                uObject = SimpleAsteroid4Controller.CreateSimpleAsteroid4();
                break;
            case UniverseObjectType.StartZone:
                uObject = StartZoneController.CreateStartZone();
                break;
            case UniverseObjectType.WinZone:
                uObject = WinZoneController.CreateWinZone();
                break;
            case UniverseObjectType.Star:
                uObject = ScorePointController.CreateScorePoint();
                break;
            case UniverseObjectType.Accelerator:
                uObject = AcceleratorPowerUpController.CreateAcceleratorPowerUp();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        uObject.StartPosition = uObjectDescriptor.Position;
        editorRoot.CurrentUniverse.Objects.Add(uObject);
        UnityEngine.Debug.Log("haaaaaaard");
    }
}