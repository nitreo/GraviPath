using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parse;
using UnityEngine;
using UniRx;


public partial class EditorRootView { 

    /// Subscribes to the property and is notified anytime the value changes.
    public override void CurrentUniverseChanged(UniverseViewModel value) {
        base.CurrentUniverseChanged(value);
    }
 

    /// Invokes SerializeExecuted when the Serialize command is executed.
    public override void SerializeExecuted() {
        base.SerializeExecuted();



    }


}


