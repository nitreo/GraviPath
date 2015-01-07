using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public partial class LevelRootViewModel {
    public override int ComputeScore()
    {
        var score = 0;
        if (CurrentTryEntry != null)
        {
            score += (int)(CurrentTryEntry.PathLength)*2;
        }
        score += BonusScore;
        return score;
    }

    public bool Initialized { get; set; }
}
