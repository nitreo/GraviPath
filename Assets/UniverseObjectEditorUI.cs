using System;
using UnityEngine;
using System.Collections;
using UniRx;
using UnityEngine.EventSystems;

public class UniverseObjectEditorUI : MonoBehaviour {


    public EventTrigger RotateHandle;
    public EventTrigger DragHandle;
    private RectTransform _thisCanvas;
    private Transform _object;
    

    public void Init()
    {

        _object = transform.GetComponentInParent<UniverseObjectView>().transform;
        _thisCanvas = GetComponent<RectTransform>();
        transform.localPosition = Vector3.zero;
        _thisCanvas.eulerAngles = Vector3.zero;
        //_thisCanvas.localScale = _object.localScale;
        DragHandle.AsObservableOfDrag().Subscribe(_ =>
        {
            var args = _ as PointerEventData;
            var pos = Camera.main.ScreenToWorldPoint(args.position);
            _object.position = new Vector3(pos.x, pos.y);
        });

        RotateHandle.AsObservableOfDrag().Subscribe(_ =>
        {
            var args = _ as PointerEventData;
            var pos = Camera.main.ScreenToWorldPoint(args.position);
            _object.localEulerAngles += new Vector3(0, 0, args.delta.x / 10);
            _thisCanvas.eulerAngles = Vector3.zero;
        });

    }
	
}
