using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UnityEngine.UI;


public partial class LevelRootGUIView {
    
    public Text ScoreText;
    public Button RestartButton;
    public Button QuitButton;
    
    public override void ScoreChanged(Int32 value) {
        base.ScoreChanged(value);
        if (value <= 4) ScoreText.text = "Score: " + 0;
        else ScoreText.text = "Score: " + value;
    }

    public override void Bind()
    {
        base.Bind();
        RestartButton.AsClickObservable()
            .Subscribe(_ => { ExecuteRestart(true); })
            .DisposeWith(this);

        QuitButton.AsClickObservable()
            .Subscribe(_ => { ExecuteToMenu(); })
            .DisposeWith(this);
    }
}
