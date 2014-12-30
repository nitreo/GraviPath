using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UniRx;

public class GenericAudioSource : MonoBehaviour {

    private static GenericAudioSource _instance;
    private AudioSource source;
    public static GenericAudioSource instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GenericAudioSource>();

                //Tell unity not to destroy this object when loading a new scene!
                //DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            //If I am the first instance, make me the Singleton
            _instance = this;
            source = GetComponent<AudioSource>();
            //DontDestroyOnLoad(this);
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }


    public AudioClip[] Pops;
    private bool allowPop = true;
    public void PlayPop()
    {
        if (!allowPop) return;
        allowPop = false;
        Observable.Timer(TimeSpan.FromMilliseconds(10)).Subscribe(_ =>
        {
            allowPop = true;
        });
        source.PlayOneShot(Pops.OrderBy(_=>UnityEngine.Random.Range(0.0f,1.0f))
            .First());
    }
}
