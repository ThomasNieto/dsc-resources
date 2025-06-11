// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

namespace Dsc.Resource;

public interface IDscResource<T>
{
    string GetSchema(Type type);
    string ToJson(T input);
    T Parse(string json);
}

public interface IGet<T> : IDscResource<T>
{
    T Get(T input);
}

public interface ISet<T> : IDscResource<T>
{
    void Set(T input);
}

public interface ISetWhatIf<T> : ISet<T>
{
    void Set(T input, bool whatIf);
}

public interface IDelete<T> : IDscResource<T>
{
    void Delete(T input);
}

public interface IDeleteWhatIf<T> : IDelete<T>
{
    void Delete(T input, bool whatIf);
}

public interface ITest<T> : IDscResource<T>
{
    bool Test(T input);
}

public interface IExport<T> : IDscResource<T>
{
    IEnumerable<T> Export();
}
