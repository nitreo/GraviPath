using UnityEngine;
using System.Collections;
using System.Threading;

public class Mover : MonoBehaviour
{

    public Vector3 Speed;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    transform.position += Speed*Time.deltaTime;
	}
}
