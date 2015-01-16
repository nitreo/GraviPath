using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


public class LevelRootController : LevelRootControllerBase {
    
    public override void InitializeLevelRoot(LevelRootViewModel levelRoot)
    {
        if (levelRoot.Initialized == true) return;
        levelRoot.Initialized = true;
        levelRoot.PlayerProperty
            .Where(p=>p!=null)
            .Subscribe(p=>NewPlayerSet(levelRoot,p))
            .DisposeWith(levelRoot);
    }

    private void NewPlayerSet(LevelRootViewModel levelRoot, PlayerViewModel playerViewModel)
    {
        playerViewModel
            .ZoneReached
            .Select(_ => playerViewModel.ZoneReached.Parameter as ZoneViewModel)
            .Where(z => z is WinZoneViewModel)
            .Subscribe(z =>
            {
                ExecuteCommand(playerViewModel.Dock,new DockDescriptor()
                {
                    Position = z.Position
                });
            })
            .DisposeWhenChanged(levelRoot.PlayerProperty)
            .DisposeWith(levelRoot);
    }

    public void AllocateTryEntry(LevelRootViewModel levelRoot)
    {
        var tryEntryViewModel = TryEntryController.CreateTryEntry();
        levelRoot.Attempts.Add(tryEntryViewModel);
        tryEntryViewModel.Target = levelRoot.Player;
        levelRoot.CurrentTryEntry = tryEntryViewModel;
        ExecuteCommand(tryEntryViewModel.Reset);
    }


    public override void Restart(LevelRootViewModel levelRoot, bool saveAttempt)
    {
        base.Restart(levelRoot, saveAttempt);
        levelRoot.BonusScore = 0;
        if (levelRoot.Player != null)
        {
            ExecuteCommand(levelRoot.Player.Reset);

            var lastTry = levelRoot.Attempts.LastOrDefault<TryEntryViewModel>();
            if (lastTry != null && (!saveAttempt || lastTry.PathLength < 1) )
            {
                levelRoot.Attempts.Remove(lastTry);
            }
             
            AllocateTryEntry(levelRoot);
        }
        if(levelRoot.Universe!=null)
        this.ExecuteCommand(levelRoot.Universe.Reset);
    }


    public override void LoadUniverse(LevelRootViewModel levelRoot, UniverseViewModel arg)
    {

        foreach (var attempt in levelRoot.Attempts.ToList())
        {
            levelRoot.Attempts.Remove(attempt);
        }

        if (arg == null)
        {
            arg = UniverseController.CreateInitialUniverse();
        }

        base.LoadUniverse(levelRoot, arg);
        
        arg.IsEditable = false;
        levelRoot.Universe = arg;
        ExecuteCommand(levelRoot.Restart,false);
    
    }
}
