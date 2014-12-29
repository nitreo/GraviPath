using System;
using UniRx;

public class EditorRootController : EditorRootControllerBase
{
    [Inject] public SimplePlanet1Controller SimplePlanet1Controller;
    [Inject] public SimplePlanet2Controller SimplePlanet2Controller;
    [Inject]
    public SimpleAsteroid1Controller SimpleAsteroid1Controller;
    [Inject]
    public SimpleAsteroid2Controller SimpleAsteroid2Controller;
    [Inject]
    public SimpleAsteroid3Controller SimpleAsteroid3Controller;

    public override void InitializeEditorRoot(EditorRootViewModel editorRoot)
    {
        editorRoot.NewUniverseDataProperty.Where(editor => editor != null)
            .Subscribe(editor => { EditorChanged(editorRoot, editor); });


        editorRoot.AvailableUniverses.Clear();
        UniverseRepository.GetLatestPaged(10, 0).Subscribe(uni => { editorRoot.AvailableUniverses.Add(uni); });
    }

    private void EditorChanged(EditorRootViewModel editorRoot, NewUniverseSubEditorViewModel editor)
    {
        editorRoot.NewUniverseData.Create.Subscribe(_ =>
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
        UniverseRepository.SaveUniverse(editorRoot.CurrentUniverse).Delay(TimeSpan.FromSeconds(2)).Subscribe(_ =>
        {
            editorRoot.AvailableUniverses.Clear();
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
            default:
                throw new ArgumentOutOfRangeException();
        }

        uObject.Position = arg.Position;
        editorRoot.CurrentUniverse.Objects.Add(uObject);
    }
}