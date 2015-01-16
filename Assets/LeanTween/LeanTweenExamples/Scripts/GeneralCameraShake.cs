using UnityEngine;
using System.Collections;

public class GeneralCameraShake : MonoBehaviour {

	LTDescr tween1;
	LTDescr tween2;

	void Start () {
		// tween1 = LeanTween.rotateAroundLocal( gameObject, Vector3.up, 5f, 2.2f).setEase( LeanTweenType.easeShake ).setLoopClamp().setRepeat(-1);
		tween2 = LeanTween.rotateAroundLocal( gameObject, Vector3.right, 3f, 0.25f).setEase( LeanTweenType.easeShake ).setLoopClamp().setRepeat(-1).setDelay(0.05f);

		LeanTween.delayedCall(gameObject, 3f, stopShake);
	}

	void stopShake(){
		LeanTween.value(gameObject, 1f, 4.0f, 2.0f).setOnUpdate( 
			(float val)=>{
				//tween1.setTime(val*0.2f);
				tween2.setTime(val*0.25f);

				//tween1.setTo(Vector3.up*val);
				tween2.setTo(Vector3.right*(1.0f / val));
			}
		).setEase(LeanTweenType.easeInSine).setOnComplete(
			()=>{
				//tween1.setRepeat(1);
				tween2.setRepeat(1);
				Debug.Log("finished");
			}
		);
		
	}

}


public static class LTCameraExtensions
{

    public static void ShakeOrtho(this Camera camera, float time)
    {

        LTDescr tween1;
        LTDescr tween2;

        tween1 = LeanTween.moveLocal(camera.gameObject, new Vector3(0,0.6f,0), 0.2f)
            .setEase(LeanTweenType.easeShake)
            .setDelay(0.05f);
        tween2 = LeanTween.moveLocal(camera.gameObject, new Vector3(0.6f,0,0), 0.2f)
            .setEase(LeanTweenType.easeShake)
            .setDelay(0.05f);
        
       
        
    }

}