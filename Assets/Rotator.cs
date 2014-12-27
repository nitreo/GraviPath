using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{

    public Vector3 Speed;
	
	// Update is called once per frame
	void Update ()
	{

	    transform.localEulerAngles += Speed;

	}
}
