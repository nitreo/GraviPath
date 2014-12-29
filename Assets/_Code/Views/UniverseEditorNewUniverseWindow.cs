using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UnityEngine.UI;


public partial class UniverseEditorNewUniverseWindow
{



    public GameObject CreateNewUniverseWindow;

    public InputField NewUniverseNameInputField;
    public Text NewUniverseNameInputLabel;
    public Text NewUniverseNameInputMessage;

    public InputField NewUniverseDescriptionInputField;
    public Text NewUniverseDescriptionInputLabel;
    public Text NewUniverseDescriptionInputMessage;

    public Button CreateNewUniverseButton;
    public Button NopeButton;

    public override void Bind()
    {
        base.Bind();
        CreateNewUniverseButton
            .AsClickObservable()
            .Subscribe(_ => { ExecuteCreateUniverse(); })
            .DisposeWith(this);

        NewUniverseNameInputField.AsValueChangedObservable().Subscribe(val =>
        {
            NewUniverseSubEditor.Name = val;
        });

        NopeButton.AsClickObservable().Subscribe(_ =>
        {
            NewUniverseSubEditor.IsActive = false;
        });

    }

    /// Subscribes to the property and is notified anytime the value changes.
    public override void IsActiveChanged(Boolean value) {
        base.IsActiveChanged(value);
        CreateNewUniverseWindow.SetActive(value);
    }
    
    /// Subscribes to the property and is notified anytime the value changes.
    public override void NameChanged(String value) {
        base.NameChanged(value);
    }
    
    /// Subscribes to the property and is notified anytime the value changes.
    public override void DescriptionChanged(String value) {
        base.DescriptionChanged(value);
    }
    
    /// Subscribes to the property and is notified anytime the value changes.
    public override void IsValidChanged(Boolean value) {
        base.IsValidChanged(value);
    }
    
  
}
