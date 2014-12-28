using UnityEngine;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

public class SineMover : MonoBehaviour
{

    public float Amp;
    public float Speed = 6f;
    public bool ModifyUpVector;
    public Vector3 Velocity;


    private float _t = 0;
    
	// Update is called once per frame
	void Update ()
	{
        var x = Amp * Mathf.Sin(_t);        
        transform.localPosition += new Vector3(x, 0, 0);
        _t += Speed *Time.deltaTime;
	}
}
