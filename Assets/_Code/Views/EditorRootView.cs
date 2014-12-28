using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parse;
using UnityEngine;
using UniRx;
using UnityEngine.UI;


public partial class EditorRootView
{ 

    /// Subscribes to collection modifications.  Add & Remove methods are invoked for each modification.
    public override void AvailableUniversesAdded(UniverseViewModel item) {
        base.AvailableUniversesAdded(item);
    }
    
    /// Subscribes to collection modifications.  Add & Remove methods are invoked for each modification.
    public override void AvailableUniversesRemoved(UniverseViewModel item) {
        base.AvailableUniversesRemoved(item);
    }



    private ViewBase _currentUniverseView = null;

    /// Subscribes to the property and is notified anytime the value changes.
    public override void CurrentUniverseChanged(UniverseViewModel value) {
        base.CurrentUniverseChanged(value);

        //Create new for the new universe:


        

        if (value != null)
        {
            InstantiateView(value);
        }


    }


    public void OnGUI()
    {
        foreach (var availableUniverse in EditorRoot.AvailableUniverses)
        {
            if (GUILayout.Button(availableUniverse.Name))
            {
                ExecuteLoadUniverse(availableUniverse);
            }
        }
    }

    public Button CreateNewUniverseButton;

    public override void Bind()
    {
        base.Bind();
        CreateNewUniverseButton.AsClickObservable().Subscribe(_ =>
        {
            ExecuteToggleNewUniverseSubEditor();
        });
    }
}


