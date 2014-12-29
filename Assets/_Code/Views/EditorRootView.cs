using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parse;
using UnityEngine;
using UniRx;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public partial class EditorRootView
{

    private UniverseView _currentUniverseView = null;

    public Canvas MainCanvas;
    public Button SaveButton;
    public Button ExitButton;
    public RectTransform UniversesList;    
    public GameObject UniverseListItem;
    private readonly Dictionary<UniverseViewModel, GameObject> AvailableUniverse2ButtonMap = new Dictionary<UniverseViewModel, GameObject>();

    /// Subscribes to collection modifications.  Add & Remove methods are invoked for each modification.
    public override void AvailableUniversesAdded(UniverseViewModel item) {
        base.AvailableUniversesAdded(item);
        var uiItem = Instantiate(UniverseListItem) as GameObject;

        var textBox = uiItem.GetComponentInChildren<Text>();
        textBox.text = item.Name;

        uiItem.transform.SetParent(UniversesList, false);

        AvailableUniverse2ButtonMap.Add(item, uiItem);
        uiItem.GetComponent<Button>().AsClickObservable().Subscribe(_ =>
        {
            ExecuteLoadUniverse(item);
        }).DisposeWith(this);
    }
    
    /// Subscribes to collection modifications.  Add & Remove methods are invoked for each modification.
    public override void AvailableUniversesRemoved(UniverseViewModel item) {
        base.AvailableUniversesRemoved(item);
        var uiItem = AvailableUniverse2ButtonMap[item];
        AvailableUniverse2ButtonMap.Remove(item);
        Destroy(uiItem.gameObject);
    }
    
    /// Subscribes to the property and is notified anytime the value changes.
    public override void CurrentUniverseChanged(UniverseViewModel value) {
        base.CurrentUniverseChanged(value);

        if(_currentUniverseView != null)
            GameObject.Destroy(_currentUniverseView.gameObject);

        if (value != null)
        {
            _currentUniverseView = InstantiateView(value) as UniverseView;
            SaveButton.gameObject.SetActive(true);
        }
        else
        {
            SaveButton.gameObject.SetActive(false);
        }
    }


    public void OnGUI()
    {
/*        foreach (var availableUniverse in EditorRoot.AvailableUniverses)
        {
            if (GUILayout.Button(availableUniverse.Name))
            {
                ExecuteLoadUniverse(availableUniverse);
            }
        }*/
    }

    public Button CreateNewUniverseButton;

    public override void Bind()
    {
        base.Bind();
        
        CreateNewUniverseButton.AsClickObservable().Subscribe(_ => { ExecuteToggleNewUniverseSubEditor(); });
        SaveButton.AsClickObservable().Subscribe(_ => { ExecuteSaveCurrentUniverse(); });

        ExitButton.AsClickObservable().Subscribe(_ =>
        {
            ExecuteToMenu();
        });

    }

   


}


