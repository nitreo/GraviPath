using UnityEngine;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using UniRx;

public class SineMover : MonoBehaviour
{

    public float Amp;
    public float Freq;

    private float _t = 0;



    void Start()
    {

        var pivotPos = transform.localPosition;

        Observable.EveryUpdate().Subscribe(_ =>
        {
            var x = Amp * Mathf.Sin(Time.time * 2 * Mathf.PI * Freq);
            transform.localPosition = new Vector3(pivotPos.x+x,pivotPos.y,0);
        
        }).DisposeWith(gameObject);
    }

}
