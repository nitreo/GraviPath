using UnityEngine;
using System.Collections;
using Thinksquirrel.Phys2D;
using UnityEngine.UI;

public class UniverseGravityObjectEditorUI : UniverseObjectEditorUI
{

    public Image GravityZoneImage;

    public override void Init()
    {
        base.Init();
        var component = GetComponentInParent<GravityController2DExt>();
        var rad = component.maxRadius;
        GravityZoneImage.transform.localScale = new Vector3(rad,rad,1);
    }
}
