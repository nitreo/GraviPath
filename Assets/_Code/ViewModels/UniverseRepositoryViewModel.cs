using System;
using System.Text;
using System.Threading.Tasks;
using Parse;
using UniRx;
using UnityEngine;

public partial class UniverseRepositoryViewModel
{
    [Inject] public UniverseController UniverseController;

    public IObservable<UniverseViewModel> GetUniverseByName(string name)
    {
        var subject = new ReplaySubject<UniverseViewModel>();
        var query = new ParseQuery<UniverseMetaData>()
            .WhereEqualTo("Name", name);

        query.FirstAsync().ContinueWith(t =>
        {
            subject.OnNext(CreateUniverseFromModel(t.Result));
            subject.OnCompleted();
        });

        return subject;
    }




    public IObservable<UniverseViewModel> GetLatestPaged(int perPage, int page)
    {
        var subject = new ReplaySubject<UniverseViewModel>();
        var subjectInternal = new ReplaySubject<UniverseMetaData>();
        var query = new ParseQuery<UniverseMetaData>()
            .OrderByDescending("createdAt")
            .Skip(perPage*page)
            .Limit(perPage);

        query.FindAsync().ContinueWith(t =>
        {
            foreach (var meta in t.Result)
            {
                subjectInternal.OnNext(meta);
            }
            subjectInternal.OnCompleted();
        });

        return subjectInternal.ObserveOnMainThread().Select(meta => CreateUniverseFromModel(meta)) as IObservable<UniverseViewModel>;
    }

    public IObservable<UniverseViewModel> GetByFirstPartOfNamePaged(string startsWith, int perPage, int page)
    {
        var subject = new ReplaySubject<UniverseViewModel>();
        var query = new ParseQuery<UniverseMetaData>()
            .WhereStartsWith("Name", startsWith)
            .Skip(perPage*page)
            .Limit(perPage);

        query.FindAsync().ContinueWith(t =>
        {
            foreach (var meta in t.Result) subject.OnNext(CreateUniverseFromModel(meta));
            subject.OnCompleted();
        });

        return subject;
    }

    public Task SaveUniverse(UniverseMetaData meta)
    {
        return meta.SaveAsync();
    }

    public Task UpdateUniverse(UniverseMetaData meta)
    {
        return meta.SaveAsync();
    }

    private IObservable<UniverseMetaData> GetMetaById(string id)
    {
        var subject = new ReplaySubject<UniverseMetaData>();

        new ParseQuery<UniverseMetaData>()
            .GetAsync(id)
            .ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    foreach (var e in t.Exception.InnerExceptions)
                    {
                        var parseException = (ParseException) e;
                        if (parseException.Code == ParseException.ErrorCode.ObjectNotFound)
                        {
                            subject.OnNext(null);
                            subject.OnCompleted();
                        }
                        else
                        {
                            Debug.Log("Error message " + parseException.Message);
                            Debug.Log("Error code: " + parseException.Code);
                            throw new Exception(" SOMETHING IS WRONG");
                        }
                    }
                }
                else
                {
                    subject.OnNext(t.Result);
                    subject.OnCompleted();
                }
            });

        return subject;
    }

    public IObservable<object> SaveUniverse(UniverseViewModel viewModel)
    {
        var subject = new ReplaySubject<object>();
        var newUniverse = false;

        GetMetaById(viewModel.Identifier).ObserveOnMainThread()
            .Subscribe(meta =>
            {
                if (meta == null)
                {
                    meta = new UniverseMetaData();
                    newUniverse = true;
                }

                meta.Name = viewModel.Name;
                meta.UniverseData = SerializeUniverse(viewModel);
                meta.SaveAsync().ContinueWith(_ =>
                {
                    if (newUniverse)
                    {
                        viewModel.Identifier = meta.ObjectId;
                    }
                    subject.OnNext(null);
                    subject.OnCompleted();
                });

            });

        return subject;
    }

    public UniverseMetaData CreateModelFromUniverse(UniverseViewModel universe)
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
        universe.Write(stream);
        storage.Save(stream);

        var meta = new UniverseMetaData();

        //Create new parse object


        //Setup metadata
        meta.Name = universe.Name;
        meta.UniverseData = storage.ToString();


        return meta;
    }

    public UniverseViewModel CreateUniverseFromModel(UniverseMetaData meta)
    {
        //Prepare typeresolver
        var activeSceneManager = GameManager.ActiveSceneManager;
        if (activeSceneManager == null) throw new Exception("No scenemanager is active");

        //Prepare stream
        var stream = new JsonStream();
        stream.DeepSerialize = true;
        stream.TypeResolver = activeSceneManager;
        stream.DependencyContainer = GameManager.Container;
        stream.Load(Encoding.UTF8.GetBytes(meta.UniverseData));

        var viewModel = UniverseController.CreateUniverse();
        viewModel.Read(stream);

        //!!! Save id
        viewModel.Identifier = meta.ObjectId;

        return viewModel;
    }

    public string SerializeUniverse(UniverseViewModel viewmodel)
    {
        Debug.Log("Here?");

        var activeSceneManager = GameManager.ActiveSceneManager;
        if (activeSceneManager == null) throw new Exception("No scenemanager is active");

        viewmodel.Author = " UNDEF ";

        //Prepare stream
        var stream = new JsonStream
        {
            DeepSerialize = true,
            TypeResolver = activeSceneManager
        };

        //Prepare storage
        var storage = new StringSerializerStorage();
        //Serialize
        viewmodel.Write(stream);
        storage.Save(stream);

        return storage.ToString();
    }
}