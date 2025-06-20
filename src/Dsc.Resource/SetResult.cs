// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

namespace Dsc.Resource;

public sealed class SetResult<T>
{
    public T ActualState { get; }

    public HashSet<string>? ChangedProperties { get; set; }

    public SetResult(T actualState)
    {
        ActualState = actualState;
    }
}
