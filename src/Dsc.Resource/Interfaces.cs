// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

namespace Dsc.Resource;

public interface IDscResource<T>
{
    string GetSchema(Type type);
    string ToJson(T item);
    T Parse(string json);
}

public interface IGettable<T> : IDscResource<T>
{
    T Get(T item);
}

public interface ISettable<T> : IDscResource<T>
{
    void Set(T item);
}

public interface ISettableWhatIf<T> : ISettable<T>
{
    void Set(T item, bool whatIf);
}

public interface IDeletable<T> : IDscResource<T>
{
    void Delete(T item);
}

public interface IDeletableWhatIf<T> : IDeletable<T>
{
    void Delete(T item, bool whatIf);
}

public interface ITestable<T> : IDscResource<T>
{
    bool Test(T item);
}

public interface IExportable<T> : IDscResource<T>
{
    IEnumerable<T> Export();
}
