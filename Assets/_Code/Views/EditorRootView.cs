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

    /// Subscribes to collection modifications.  Add & Remove methods are invoked for each modification.
    public override void AvailableUniversesAdded(UniverseViewModel item) {
        base.AvailableUniversesAdded(item);
    }
    
    /// Subscribes to collection modifications.  Add & Remove methods are invoked for each modification.
    public override void AvailableUniversesRemoved(UniverseViewModel item) {
        base.AvailableUniversesRemoved(item);
    }


    public EventTrigger Planet1Button;
    public GameObject DragThumbnailPrefab;
    public GameObject UniverseObjectHandles;
    public Canvas MainCanvas;
    public GameObject Toolbar;
    private UniverseView _currentUniverseView = null;
    public Button SaveButton;

    /// Subscribes to the property and is notified anytime the value changes.
    public override void CurrentUniverseChanged(UniverseViewModel value) {
        base.CurrentUniverseChanged(value);


        if(_currentUniverseView != null)
            GameObject.Destroy(_currentUniverseView.gameObject);


        if (value != null)
        {
            _currentUniverseView = InstantiateView(value) as UniverseView;
            Toolbar.SetActive(true);
            SaveButton.gameObject.SetActive(true);
        }
        else
        {
            Toolbar.SetActive(false);
            SaveButton.gameObject.SetActive(false);
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

        SaveButton.AsClickObservable().Subscribe(_ =>
        {
            ExecuteSaveCurrentUniverse();
        });

        Planet1Button.AsObservableOfBeginDrag().Subscribe(_ =>
        {

            var dragBeginArgs = _ as PointerEventData;
            var newThumb = Instantiate(DragThumbnailPrefab) as GameObject;

            //OMG

            var trans = newThumb.GetComponent<RectTransform>();
            var thumber = newThumb.GetComponent<UniverseObjectsToolbarDragThumbnail>();
            thumber.SetPlanet1();
            trans.SetParent(MainCanvas.transform,false);
            trans.position = Planet1Button.transform.position;
            //trans.pivot = new Vector2(0.5f,0.5f);
            

            var dragDisp = Planet1Button.AsObservableOfDrag().Subscribe(__ =>
            {
                var dragArgs = __ as PointerEventData;
                var point = Camera.main.ScreenToWorldPoint(dragArgs .position);
                trans.position = new Vector3(point.x,point.y,0);

            }).DisposeWith(newThumb);


            var dragEndDisp = Planet1Button.AsObservableOfEndDrag().Subscribe(__ =>
            {
                var dragArgs = __ as PointerEventData;
                GameObject.Destroy(newThumb);

                var descriptor = new UniverseObjectDescriptor()
                {
                    Name = "Planet1",
                    Position = trans.position
                };

                ExecuteAddUniverseObject(descriptor);

            }).DisposeWith(newThumb);

        });
        Planet1Button.AsObservableOfDrag().Subscribe(_ =>
        {
            var args = _ as PointerEventData;


        });

    }
}


