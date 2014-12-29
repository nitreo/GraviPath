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
    public Image CreateDragHandle;
    public Canvas MainCanvas;
    public GameObject Toolbar;
    public Button SaveButton;
    public CreateObjectButtonDescriptor[] CreateObjectToolbarButtons;

    /// Subscribes to collection modifications.  Add & Remove methods are invoked for each modification.
    public override void AvailableUniversesAdded(UniverseViewModel item) {
        base.AvailableUniversesAdded(item);
    }
    
    /// Subscribes to collection modifications.  Add & Remove methods are invoked for each modification.
    public override void AvailableUniversesRemoved(UniverseViewModel item) {
        base.AvailableUniversesRemoved(item);
    }
    
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

        foreach (var button in CreateObjectToolbarButtons)
        {
            RegisterToolbarDragButton(button);
        }

    }

    public void RegisterToolbarDragButton(CreateObjectButtonDescriptor button)
    {
        var trigger = button.GetComponent<EventTrigger>();
        trigger.AsObservableOfBeginDrag().Subscribe(_ =>
        {
            CreateObjectHandlePicked(CreateDragHandle,button);
        });

        trigger.AsObservableOfDrag().Subscribe(_ =>
        {
            var dragArgs = _ as PointerEventData;
            var point = Camera.main.ScreenToWorldPoint(dragArgs.position);
            CreateObjectHandleDragged(CreateDragHandle,point,button);                
        });
        trigger.AsObservableOfEndDrag().Subscribe(_ =>
        {
            CreateObjectHandleDropped(CreateDragHandle,button);                
        });
    }

    public void CreateObjectHandlePicked(Image handle, CreateObjectButtonDescriptor source)
    {
        handle.rectTransform.position = source.transform.position;
        handle.overrideSprite = source.DragSprite;

        handle.gameObject.SetActive(true);
    }

    public void CreateObjectHandleDragged(Image handle, Vector3 newPosition,CreateObjectButtonDescriptor source)
    {
        handle.rectTransform.position = new Vector3(newPosition.x, newPosition.y, 0);
    }

    public void CreateObjectHandleDropped(Image handle, CreateObjectButtonDescriptor source)
    {
        handle.gameObject.SetActive(false);
        CreateObject(source.ObjectType,handle.transform.position);
    }

    public void CreateObject(UniverseObjectType type, Vector3 position)
    {
        var descriptor = new UniverseObjectDescriptor()
        {
            Type = type,
            Position = position
        };
        ExecuteAddUniverseObject(descriptor);
    }


}


