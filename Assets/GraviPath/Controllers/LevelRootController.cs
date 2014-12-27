using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


public class LevelRootController : LevelRootControllerBase {
    
    public override void InitializeLevelRoot(LevelRootViewModel levelRoot)
    {

    }

    public void AllocateTryEntry(LevelRootViewModel levelRoot)
    {
        var tryEntryViewModel = TryEntryController.CreateTryEntry();
        levelRoot.Attempts.Add(tryEntryViewModel);
        tryEntryViewModel.Target = levelRoot.Player;
        levelRoot.CurrentTryEntry = tryEntryViewModel;
    }


    public override void Restart(LevelRootViewModel levelRoot, bool saveAttempt)
    {
        base.Restart(levelRoot, saveAttempt);
        if (levelRoot.Player != null)
        {
            this.ExecuteCommand(levelRoot.Player.Reset);
            if (!saveAttempt && levelRoot.Attempts.Count > 0)
            {
                var lastAttempt = levelRoot.Attempts.Last<TryEntryViewModel>();
                levelRoot.Attempts.Remove(lastAttempt);
            }
            AllocateTryEntry(levelRoot);
        }
    }
}
