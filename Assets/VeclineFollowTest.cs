using System;
using UnityEngine;
using System.Collections;
using UniRx;
using UnityEditor;
using Vectrosity;

public class VeclineFollowTest : MonoBehaviour
{

    private VectorLine line;

    void Start()
    {
        line = VectorLine.SetLine(Color.yellow,new Vector3[]{new Vector3(0,0,0),new Vector3(1,1,0),  });
    }

}
