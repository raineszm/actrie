using System.Diagnostics.CodeAnalysis;

namespace AcTrie;

public readonly record struct Option<T>
{
    public T Value { get; private init; } = default!;
    public bool IsSome { get; private init; }
    public bool IsNone => !IsSome;
    
    public Option() {}
    public Option(T value)
    {
        Value = value;
        IsSome = true;
    }
    
    public static implicit operator Option<T>(T value) => new(value);

    public bool TryGetValue([MaybeNullWhen(returnValue: false)] out T value)
    {
        if (IsSome)
        {
            value = Value;
            return true;
        }

        value = default;
        return false;
    }

}