using System.Collections.Generic;
using Parse;
using UniRx;


public partial class UniverseRepositoryViewModel
{
    public IObservable<UniverseMetaData> GetUniverseMetaByName(string name)
    {
        var subject = new ReplaySubject<UniverseMetaData>();
        var query = new ParseQuery<UniverseMetaData>()
            .WhereEqualTo("Name", name);

        query.FirstAsync().ContinueWith(t =>
        {
            subject.OnNext(t.Result);
            subject.OnCompleted();

        });
        return subject;

    }

    public IObservable<UniverseMetaData> GetLatestPaged(int perPage, int page)
    {
        var subject = new ReplaySubject<UniverseMetaData>();
        var query = new ParseQuery<UniverseMetaData>()
            .OrderByDescending("createdAt")
            .Skip(perPage * page)
            .Limit(perPage);
        query.FindAsync().ContinueWith(t =>
        {
            IEnumerable<UniverseMetaData> metas =t.Result;

            foreach (var meta in metas)
            {
                subject.OnNext(meta);
            }

            subject.OnCompleted();
        });


        return subject;
    }

    public IObservable<UniverseMetaData> GetByLikeNamePaged(string startsWith, int perPage, int page)
    {
        var subject = new ReplaySubject<UniverseMetaData>();
        var query = new ParseQuery<UniverseMetaData>()
            .WhereStartsWith("Name", startsWith)
            .Skip(perPage * page)
            .Limit(perPage);

            query.FindAsync().ContinueWith(t =>
            {
                IEnumerable<UniverseMetaData> metas =  t.Result;

                foreach(var meta in metas)
                {
                    subject.OnNext(meta);
                }

                subject.OnCompleted();
            });



        return subject;
    }


}


[ParseClassName("Universe")]
public class UniverseMetaData : ParseObject
{

    [ParseFieldName("Author")]
    public string Author
    {
        get { return GetProperty<string>("Author"); }
        set { SetProperty<string>(value, "Author"); }
    }

    [ParseFieldName("Name")]
    public string Name
    {
        get { return GetProperty<string>("Name"); }
        set { SetProperty<string>(value, "Name"); }
    }

    [ParseFieldName("RawJson")]
    public string Data
    {
        get { return GetProperty<string>("RawJson"); }
        set { SetProperty<string>(value, "RawJson"); }
    }

}
