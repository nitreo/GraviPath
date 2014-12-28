using System;
using Parse;

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

    [ParseFieldName("UniverseData")]
    public string UniverseData
    {
        get { return GetProperty<string>("UniverseData"); }
        set { SetProperty<string>(value, "UniverseData"); }
    }

}