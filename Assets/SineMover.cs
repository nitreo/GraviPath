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
    private Vector3 _prevPosition = new Vector3();
    private Vector3 _prevLocalPosition = new Vector3();

	// Update is called once per frame
	void Update ()
	{
        var x = Amp * Mathf.Sin(_t);
        
        var velocityY = transform.position.y - _prevPosition.y;
        var velocityX = transform.localPosition.x - _prevLocalPosition.x;
        transform.localPosition += new Vector3(x, 0, 0);
        _t += Speed *Time.deltaTime;
	}
}
