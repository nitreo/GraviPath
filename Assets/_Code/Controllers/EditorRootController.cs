using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


public class EditorRootController : EditorRootControllerBase
{
    


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
        UniverseRepository.SaveUniverse(editorRoot.CurrentUniverse);


        editorRoot.AvailableUniverses.Clear();
        UniverseRepository.GetLatestPaged(10, 0).Subscribe(uni =>
        {
            editorRoot.AvailableUniverses.Add(uni);
        });
    }

}
