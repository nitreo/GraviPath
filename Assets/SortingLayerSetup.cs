using UnityEngine;
using System.Collections;

public class SortingLayerSetup : MonoBehaviour {


    void Start(){
        var l = GetComponent<LineRenderer>();
        l.sortingLayerName = "Gameplay";
        l.sortingOrder = -1;
    }

}
