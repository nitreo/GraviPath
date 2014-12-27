using UnityEngine;
using System.Collections;

public class DirectionIndicator : MonoBehaviour
{

    public Transform Tip;
    public LineRenderer Base;

    public void SetDirection(Vector3 dir)
    {
        var tipPoint = transform.position + dir/2;
        var backPoint = transform.position - dir/2;
        Tip.up = dir.normalized;
        Tip.position = tipPoint;
        Base.SetPosition(1,transform.position);
        Base.SetPosition(0,tipPoint);
    }
        
}
