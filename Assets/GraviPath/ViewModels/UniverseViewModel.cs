using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parse;
using UniRx;
using UnityEngine;


public partial class UniverseViewModel {


    public void SaveUniverse()
    {

        //Prepare typeresolver
        var activeSceneManager = GameManager.ActiveSceneManager;
        if (activeSceneManager == null) throw new Exception("No scenemanager is active");

        //Prepare stream
        var stream = new JsonStream();
        stream.DeepSerialize = true;
        stream.TypeResolver = activeSceneManager;

        //Prepare storage
        var storage = new StringSerializerStorage();
        
        //Serialize
        Write(stream);
        storage.Save(stream);

        //Create new parse object
        ParseObject testObject = new ParseObject("Universe");
        
        //Setup metadata
        testObject["Name"] = this.Name;
        testObject["RawJson"] = storage.ToString();
        
        //Upload universe
        testObject.SaveAsync();
    }

    public void LoadUniverse(string name)
    {
        //Prepare typeresolver
        var activeSceneManager = GameManager.ActiveSceneManager;
        if (activeSceneManager == null) throw new Exception("No scenemanager is active");

        //Prepare stream
        var stream = new JsonStream();
        stream.DeepSerialize = true;
        stream.TypeResolver = activeSceneManager;
        stream.DependencyContainer = GameManager.Container;
        var query = ParseObject.GetQuery("Universe")
        .WhereEqualTo("Name", name);
       
        query.FirstAsync().ContinueWith(t =>
        {
            Debug.Log(t.IsFaulted);
            Debug.Log(t.IsCanceled);
            Debug.Log(t.IsCompleted);

            if (t.IsCompleted)
            {
                try
                {
                    string result = (string) t.Result["RawJson"];
                    //Debug.Log("Reading " + result);
                    stream.Load(Encoding.UTF8.GetBytes(result));
                
                    MainThreadDispatcher.Post(() =>
                    {
                        Read(stream);
                                
                    });
                
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
                
            }
            /* else if (t.IsCanceled)
            {

            }
            else
            {
                Debug.LogError("Failed to load universe "+name);
            }*/



        });


        //If we are ready Unload previous Universe
       
        //Load new universe

        

    }
}
