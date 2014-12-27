using UnityEngine;
using System.Collections;
using Parse;


public class SubclassRegister : MonoBehaviour {


    public static bool Initialized = false;

    void Awake()
    {
        if (!Initialized)
        {
            RegisterParseComSubclasses();
            Initialized = true;
        }
    }

    public void RegisterParseComSubclasses()
    {
       ParseObject.RegisterSubclass<UniverseMetaData>();
    }

}
