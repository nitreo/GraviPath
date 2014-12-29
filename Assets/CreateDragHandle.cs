using UnityEngine;
using System.Collections;
using System.Threading;
using UnityEngine.UI;

public class CreateDragHandle : MonoBehaviour {

    private Image thisImage;
    public Sprite Planet1;
    void Awake()
    {
        thisImage = GetComponent<Image>();
    }

    public void SetPlanet1()
    {
        thisImage.overrideSprite = Planet1;
    }


}
