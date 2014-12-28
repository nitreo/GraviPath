using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using PathologicalGames;
using UniRx;

public class DemoParallaxLayer : MonoBehaviour
{

    public Transform ReferencePoint;
    public float XSpread;
    public float DespawnDistance;
    public int MaxObjects;
    public Vector3 MovementSpeed;
    public SpawnPool Pool;
    public GameObject[] AllowedPrefabs;
    public HashSet<Transform> CurrentObjects = new HashSet<Transform>();
    public float SpawnChance;
    public bool Prewarm;
    public int ZIndex;
    public bool ApplyRandomRotation = true;
    public float Cooldown = 0.5f;
    private double cooldownCounter;
    public float PrewarmStep = 0.1f;
    void Start()
    {
        Observable.Interval(TimeSpan.FromMilliseconds(50)).Subscribe(_ =>
        {
            if (cooldownCounter <= 0 && UnityEngine.Random.Range(0.0f, 1.0f)<SpawnChance)
            {
                //Spawn object
                var prefab = AllowedPrefabs.OrderBy(p => rnd).First();
                var allocatedObject = Pool.Spawn(prefab);
                allocatedObject.transform.parent = transform;
                allocatedObject.position = ReferencePoint.position + new Vector3(urnd * XSpread, DespawnDistance - 0.1f, ZIndex);
                if(ApplyRandomRotation) allocatedObject.localEulerAngles = new Vector3(0, 0, rnd * 36);
                CurrentObjects.Add(allocatedObject);
                cooldownCounter = Cooldown;
            }

         

        }).DisposeWith(gameObject);
 
        Observable.Interval(TimeSpan.FromMilliseconds(1000)).Subscribe(_ =>
        {
            var toDestroy = CurrentObjects.Where(c => Mathf.Abs(c.transform.position.y - ReferencePoint.position.y) > DespawnDistance).ToList();
            CurrentObjects.RemoveWhere(c => toDestroy.Contains(c));
            toDestroy.ForEach(transform1 =>
                Pool.Despawn(transform1));

 

        }).DisposeWith(gameObject);

        if (Prewarm)
        {
            for (float i = DespawnDistance; i > -DespawnDistance; i -= PrewarmStep)
            {
                if (UnityEngine.Random.Range(0.0f, 1.0f) < SpawnChance)
                {
                    //Spawn object
                    var prefab = AllowedPrefabs.OrderBy(p => rnd).First();
                    var allocatedObject = Pool.Spawn(prefab);
                    allocatedObject.transform.parent = transform;
                    allocatedObject.position = transform.position + new Vector3(urnd * XSpread, i, ZIndex);
                    if (ApplyRandomRotation) allocatedObject.localEulerAngles = new Vector3(0, 0, rnd * 36);
                    CurrentObjects.Add(allocatedObject);
                }
            }
        }

    }

	// Update is called once per frame
	void Update ()
	{
	    var transforms = CurrentObjects.ToArray();
	   
        for (int i = 0; i < transforms.Length; i++)
	    {
	        transforms[i].localPosition += MovementSpeed * Time.deltaTime;
	    }
       
	    if (cooldownCounter > 0) cooldownCounter -= Time.deltaTime;
	}

    private float rnd
    {
        get { return UnityEngine.Random.Range(0.0f, 10.0f); }
    }  
    
    private float urnd
    {
        get { return UnityEngine.Random.Range(-1.0f, 1.0f); }
    }
}
