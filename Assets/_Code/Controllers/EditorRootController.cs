using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


public class EditorRootController : EditorRootControllerBase
{

    [Inject] public SimplePlanet1Controller SimplePlanet1Controller;

    public override void InitializeEditorRoot(EditorRootViewModel editorRoot)
    {
        editorRoot.NewUniverseDataProperty.Where(editor => editor != null).Subscribe(editor =>
        {
            EditorChanged(editorRoot, editor);
        });


        editorRoot.AvailableUniverses.Clear();
        UniverseRepository.GetLatestPaged(10, 0).Subscribe(uni =>
        {
            editorRoot.AvailableUniverses.Add(uni);
        });

    }

    private void EditorChanged(EditorRootViewModel editorRoot, NewUniverseSubEditorViewModel editor)
    {

        editorRoot.NewUniverseData.Create.Subscribe(_ =>
        {
            this.ExecuteCommand(editorRoot.CreateNewUniverse);
            this.ExecuteCommand(editorRoot.ToggleNewUniverseSubEditor);
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
            UniverseRepository.GetLatestPaged(10, 0).Subscribe(uni =>
            {
                editorRoot.AvailableUniverses.Add(uni);
            });    
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
        if (arg.Name == "Planet1")
        {
            var planet = SimplePlanet1Controller.CreateSimplePlanet1();
            planet.Position= arg.Position;
            editorRoot.CurrentUniverse.Objects.Add(planet);
        }

    }
}
