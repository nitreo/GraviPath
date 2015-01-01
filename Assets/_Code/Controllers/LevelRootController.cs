using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


public class LevelRootController : LevelRootControllerBase {
    
    public override void InitializeLevelRoot(LevelRootViewModel levelRoot)
    {

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
    }


    public override void Restart(LevelRootViewModel levelRoot, bool saveAttempt)
    {
        base.Restart(levelRoot, saveAttempt);
        
        if (levelRoot.Player != null)
        {
            Debug.Log("sdfsdf");
            ExecuteCommand(levelRoot.Player.Reset);
                       
            if (!saveAttempt && levelRoot.Attempts.Count > 0)
            {
                var lastAttempt = levelRoot.Attempts.Last<TryEntryViewModel>();
                levelRoot.Attempts.Remove(lastAttempt);
            }
            
            AllocateTryEntry(levelRoot);
        }
    }


    public override void LoadUniverse(LevelRootViewModel levelRoot, UniverseViewModel arg)
    {
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
