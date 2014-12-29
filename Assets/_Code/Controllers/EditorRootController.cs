using System;
using System.Diagnostics;
using System.Linq;
using UniRx;

public class EditorRootController : EditorRootControllerBase
{
    [Inject] public SimplePlanet1Controller SimplePlanet1Controller;
    [Inject] public SimplePlanet2Controller SimplePlanet2Controller;
    [Inject] public SimpleAsteroid1Controller SimpleAsteroid1Controller;
    [Inject] public SimpleAsteroid2Controller SimpleAsteroid2Controller;
    [Inject] public SimpleAsteroid3Controller SimpleAsteroid3Controller;
    [Inject] public SimpleAsteroid4Controller SimpleAsteroid4Controller;
    [Inject] public StartZoneController StartZoneController;

    public override void InitializeEditorRoot(EditorRootViewModel editorRoot)
    {
        editorRoot.NewUniverseDataProperty.Where(editor => editor != null)
            .Subscribe(editor => { NewUniverseEditorChanged(editorRoot, editor); });
        editorRoot.AddUniverseObjectSubEditorProperty.Where(editor => editor != null)
            .Subscribe(editor => { AddUniverseObjectEditorChanged(editorRoot, editor); });
        editorRoot.AvailableUniverses.Clear();
        editorRoot.CurrentUniverseProperty.Subscribe(uni => { if(editorRoot.AddUniverseObjectSubEditor!=null) editorRoot.AddUniverseObjectSubEditor.IsActive = uni != null; });
        UniverseRepository.GetLatestPaged(10, 0).Subscribe(uni => { editorRoot.AvailableUniverses.Add(uni); });
    }

    private void AddUniverseObjectEditorChanged(EditorRootViewModel editorRoot, AddUniverseObjectSubEditorViewModel editor)
    {
        editorRoot.AddUniverseObjectSubEditor.Add.Subscribe(desc =>
        {
            //WHY DO I HAVE TO USE PARAMETER HERE insdeat of desc ????? HUH
            ExecuteCommand(editorRoot.AddUniverseObject,editorRoot.AddUniverseObjectSubEditor.Add.Parameter);
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

        //TODO: Setup some sort of loading screen using ContinueWith
        UniverseRepository.SaveUniverse(editorRoot.CurrentUniverse).Delay(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            foreach (var uni in editorRoot.AvailableUniverses.ToList())
            {
                editorRoot.AvailableUniverses.Remove(uni);
            }
            UniverseRepository.GetLatestPaged(10, 0).Subscribe(uni => { editorRoot.AvailableUniverses.Add(uni); });
        });
    }

    public override void LoadUniverse(EditorRootViewModel editorRoot, UniverseViewModel arg)
    {
        base.LoadUniverse(editorRoot, arg);
        arg.IsEditable = true;
        editorRoot.CurrentUniverse = arg;
    }

    public override void AddUniverseObject(EditorRootViewModel editorRoot, UniverseObjectDescriptor arg)
    {
        base.AddUniverseObject(editorRoot, arg);


        UniverseObjectViewModel uObject = null;

        switch (arg.Type)
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
            default:
                throw new ArgumentOutOfRangeException();
        }
        uObject.Position = arg.Position;
        editorRoot.CurrentUniverse.Objects.Add(uObject);
    }
}