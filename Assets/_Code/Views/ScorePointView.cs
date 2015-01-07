using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic;
using UnityEngine;
using UniRx;


public partial class ScorePointView
{

    public GameObject Art;
    public GameObject Scores;

    private HashSet<LTDescr> _tweanIds = new HashSet<LTDescr>(); 


    public override void Bind()
    {
        base.Bind();
    }

    public override void IsActiveChanged(bool value)
    {

        if (value)
        {
            _tweanIds.ToList().ForEach(t =>
            {
                t.cancel();
                _tweanIds.Remove(t);
            });

            Scores.transform.SetScaleXYZ(0,0,0);
            Art.transform.SetScaleXYZ(1,1,1);

            Scores.SetActive(false);
            Art.SetActive(true);

        }
        else
        {
            Scores.SetActive(true);
            var t1 = LeanTween.scale(Art, Vector3.zero, 0.4f);
            var t2 = LeanTween.scale(Scores, Vector3.one*2.5f, 0.4f);
            var t3 = LeanTween.scale(Scores, Vector3.zero, 0.4f)
                .setDelay(2);
                
            _tweanIds.Add(t1);
            _tweanIds.Add(t2);
            _tweanIds.Add(t3);




        }

    }

    public override void PickUpExecuted()
    {
        base.PickUpExecuted();
    }


}
