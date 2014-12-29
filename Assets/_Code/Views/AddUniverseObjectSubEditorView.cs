using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public partial class AddUniverseObjectSubEditorView { 

    /// Subscribes to the property and is notified anytime the value changes.
    public override void IsActiveChanged(Boolean value) {
        base.IsActiveChanged(value);

        Editor.SetActive(value);

    }


    public GameObject Editor;
    public Image CreateDragHandle;

    public override void Bind()
    {
        base.Bind();
        foreach (var button in Editor.GetComponentsInChildren<CreateObjectButtonDescriptor>(true))
        {
            RegisterToolbarDragButton(button);
        }
    }

    public void RegisterToolbarDragButton(CreateObjectButtonDescriptor button)
    {
        var trigger = button.GetComponent<EventTrigger>();
        trigger.AsObservableOfBeginDrag().Subscribe(_ =>
        {
            CreateObjectHandlePicked(CreateDragHandle, button);
        });

        trigger.AsObservableOfDrag().Subscribe(_ =>
        {
            var dragArgs = _ as PointerEventData;
            var point = Camera.main.ScreenToWorldPoint(dragArgs.position);
            CreateObjectHandleDragged(CreateDragHandle, point, button);
        });
        trigger.AsObservableOfEndDrag().Subscribe(_ =>
        {
            CreateObjectHandleDropped(CreateDragHandle, button);
        });
    }

    public void CreateObjectHandlePicked(Image handle, CreateObjectButtonDescriptor source)
    {
        handle.rectTransform.position = source.transform.position;
        handle.overrideSprite = source.DragSprite;

        handle.gameObject.SetActive(true);
    }

    public void CreateObjectHandleDragged(Image handle, Vector3 newPosition, CreateObjectButtonDescriptor source)
    {
        handle.rectTransform.position = new Vector3(newPosition.x, newPosition.y, 0);
    }

    public void CreateObjectHandleDropped(Image handle, CreateObjectButtonDescriptor source)
    {
        handle.gameObject.SetActive(false);
        CreateObject(source.ObjectType, handle.transform.position);
    }

    public void CreateObject(UniverseObjectType type, Vector3 position)
    {
        var descriptor = new UniverseObjectDescriptor()
        {
            Type = type,
            Position = position
        };
        ExecuteAdd(descriptor);
    }
}
