using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UnityEngine.UI;


public partial class MenuRootView
{ 



    public Button PlayButton;
    public Button EditorButton;
    public RectTransform ChooseUniverseList;
    public RectTransform ChooseUniverseMenu;
    public GameObject UniverseListItem;

    private readonly P<bool> ShowUniverseListProperty = new P<bool>(false);

    public bool ShowUniverseList
    {
        get { return ShowUniverseListProperty.Value; }
        set { ShowUniverseListProperty.Value = value; }
    }

    public override void Bind()
    {
        base.Bind();
        PlayButton.AsClickObservable().Subscribe(_ =>
        {
            ExecuteUpdateUniversesList(new UniverseListUpdateDescriptor()
            {
                Type = UniverseListUpdateType.Latest
            });
            ShowUniverseList = true;
            //ExecuteStartLevel("Level1");
        }).DisposeWith(this);

        EditorButton.AsClickObservable()
        .Subscribe(_ =>
        {
            ExecuteStartEditor();
        })
        .DisposeWith(this);

        this.BindProperty(ShowUniverseListProperty, val =>
        {
            ChooseUniverseMenu.gameObject.SetActive(val);
        });


    }


    /// Subscribes to collection modifications.  Add & Remove methods are invoked for each modification.
    public override void UniversesListAdded(UniverseViewModel item)
    {
        base.UniversesListAdded(item);
        var uiItem = Instantiate(UniverseListItem) as GameObject;

        var textBox = uiItem.GetComponentInChildren<Text>();
        textBox.text = item.Name;

        uiItem.transform.SetParent(ChooseUniverseList,false);

        AvailableUniverse2ButtonMap.Add(item,uiItem);
        uiItem.GetComponent<Button>().AsClickObservable().Subscribe(_ =>
        {
            ExecuteStartLevel(new StartLevelDescriptor()
            {
                Universe = item
            });
        }).DisposeWith(this);
    }

    /// Subscribes to collection modifications.  Add & Remove methods are invoked for each modification.
    public override void UniversesListRemoved(UniverseViewModel item)
    {
        base.UniversesListRemoved(item);
        var uiItem = AvailableUniverse2ButtonMap[item];
        AvailableUniverse2ButtonMap.Remove(item);
        Destroy(uiItem.gameObject);
    }

    private readonly Dictionary<UniverseViewModel, GameObject> AvailableUniverse2ButtonMap = new Dictionary<UniverseViewModel, GameObject>();
}
