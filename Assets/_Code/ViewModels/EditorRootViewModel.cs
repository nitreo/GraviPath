using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public partial class EditorRootViewModel {


    public void ClearUniverses()
    {
        foreach (var uni in AvailableUniverses.ToList())
            AvailableUniverses.Remove(uni);
    }


}
