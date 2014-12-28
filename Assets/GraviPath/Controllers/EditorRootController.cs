using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


public class EditorRootController : EditorRootControllerBase {
    
    public override void InitializeEditorRoot(EditorRootViewModel editorRoot)
    {
  
    }


    public override void Serialize(EditorRootViewModel editorRoot)
    {
        base.Serialize(editorRoot);
        UniverseRepository.GetUniverseMetaByName("Universe1").Subscribe(m =>
        {
            UnityEngine.Debug.Log(m.Name);
        });
    }
}
