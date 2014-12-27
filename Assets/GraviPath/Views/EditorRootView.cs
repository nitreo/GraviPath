using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        var universe = EditorRoot.CurrentUniverse;

        var storage = new StringSerializerStorage();
        var stream = new JsonStream();
        stream.DeepSerialize = true;

        stream.SerializeObject("Universe",universe);
        storage.Save(stream);

        Debug.Log(storage.ToString());

    }


}


